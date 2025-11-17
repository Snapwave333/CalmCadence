using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Captures error logs and writes rolling files under persistentDataPath/Logs, and optionally uploads.
/// Properly handles async operations and tracks upload completion.
/// </summary>
public class LogUploader : MonoBehaviour
{
    private static LogUploader _instance;
    public static LogUploader Instance => _instance;

    [Tooltip("Opt-in: if true, logs will be queued for upload.")]
    public bool uploadOptIn = false;

    [Tooltip("Upload endpoint.")]
    public string uploadUrl = "http://localhost:5000/upload";

    [Tooltip("Maximum retry attempts for failed uploads.")]
    public int maxRetries = 3;

    [Tooltip("Retry delay in seconds.")]
    public float retryDelaySecs = 2f;

    private readonly ConcurrentQueue<string> _pending = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<LogEntry> _uploadQueue = new ConcurrentQueue<LogEntry>();
    private string _logsDir;
    private CancellationTokenSource _cts;
    private Task _uploadTask;
    private bool _isShuttingDown;
    private StreamWriter _currentWriter;
    private string _currentLogDate;
    private readonly object _writerLock = new object();

    private struct LogEntry
    {
        public string Content;
        public int RetryCount;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _logsDir = Path.Combine(Application.persistentDataPath, "Logs");
        Directory.CreateDirectory(_logsDir);
        Application.logMessageReceived += OnLog;

        _cts = new CancellationTokenSource();
        _uploadTask = ProcessUploadQueueAsync(_cts.Token);
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= OnLog;
        _isShuttingDown = true;

        // Cancel and wait for upload task
        _cts?.Cancel();
        try
        {
            _uploadTask?.Wait(TimeSpan.FromSeconds(5));
        }
        catch (AggregateException) { }
        finally
        {
            _cts?.Dispose();
        }

        // Close file writer
        lock (_writerLock)
        {
            _currentWriter?.Dispose();
            _currentWriter = null;
        }

        // Flush any pending entries to disk before shutdown
        FlushPendingToDisk();
    }

    private void OnApplicationQuit()
    {
        _isShuttingDown = true;
        FlushPendingToDisk();
    }

    private void FlushPendingToDisk()
    {
        while (_pending.TryDequeue(out var entry))
        {
            try
            {
                WriteToLogFile(entry);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to flush pending log entry: {ex.Message}");
            }
        }

        lock (_writerLock)
        {
            _currentWriter?.Flush();
        }
    }

    private void OnLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            var entry = $"{DateTime.UtcNow:o} | {type} | {condition}\n{stackTrace}\n";

            // Write to file (buffered)
            WriteToLogFile(entry);

            if (uploadOptIn && !_isShuttingDown)
            {
                _pending.Enqueue(entry);
                _uploadQueue.Enqueue(new LogEntry { Content = entry, RetryCount = 0 });
            }
        }
    }

    private void WriteToLogFile(string entry)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");

        lock (_writerLock)
        {
            // Rotate log file if date changed
            if (_currentLogDate != today || _currentWriter == null)
            {
                _currentWriter?.Dispose();
                var path = Path.Combine(_logsDir, $"{today}.log");
                _currentWriter = new StreamWriter(path, append: true, Encoding.UTF8)
                {
                    AutoFlush = false
                };
                _currentLogDate = today;
            }

            _currentWriter.Write(entry);

            // Flush periodically (every 10 entries or when buffer is large)
            if (_pending.Count % 10 == 0)
            {
                _currentWriter.Flush();
            }
        }
    }

    private async Task ProcessUploadQueueAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                if (_uploadQueue.TryDequeue(out var logEntry))
                {
                    var success = await TryUploadAsync(logEntry.Content, ct);

                    if (!success && logEntry.RetryCount < maxRetries)
                    {
                        // Re-queue for retry
                        logEntry.RetryCount++;
                        _uploadQueue.Enqueue(logEntry);
                        await Task.Delay(TimeSpan.FromSeconds(retryDelaySecs * logEntry.RetryCount), ct);
                    }
                    else if (!success)
                    {
                        Debug.LogWarning($"Log upload failed after {maxRetries} retries, entry dropped");
                    }
                    else
                    {
                        // Success - remove from pending
                        _pending.TryDequeue(out _);
                    }
                }
                else
                {
                    // No items, wait before checking again
                    await Task.Delay(100, ct);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Upload queue processor error: {ex.Message}");
                await Task.Delay(1000, ct);
            }
        }
    }

    private async Task<bool> TryUploadAsync(string body, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(uploadUrl))
        {
            Debug.LogWarning("LogUploader: Upload URL is not configured");
            return false;
        }

        UnityWebRequest req = null;
        try
        {
            req = new UnityWebRequest(uploadUrl, "POST");
            byte[] data = Encoding.UTF8.GetBytes(body);
            req.uploadHandler = new UploadHandlerRaw(data);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "text/plain");
            req.timeout = 30;

            var operation = req.SendWebRequest();

            // Wait for the request to complete
            while (!operation.isDone)
            {
                if (ct.IsCancellationRequested)
                {
                    req.Abort();
                    return false;
                }
                await Task.Yield();
            }

            if (req.result == UnityWebRequest.Result.Success)
            {
                return true;
            }
            else
            {
                Debug.LogWarning($"Log upload failed: {req.error} (HTTP {req.responseCode})");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Log upload exception: {ex.GetType().Name} - {ex.Message}\n{ex.StackTrace}");
            return false;
        }
        finally
        {
            req?.Dispose();
        }
    }

    /// <summary>
    /// Gets the number of pending uploads.
    /// </summary>
    public int PendingUploadCount => _uploadQueue.Count;

    /// <summary>
    /// Manually flush all buffered logs to disk.
    /// </summary>
    public void FlushToDisk()
    {
        lock (_writerLock)
        {
            _currentWriter?.Flush();
        }
    }
}
