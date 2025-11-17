using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    private static AnalyticsManager _instance;
    public static AnalyticsManager Instance => _instance;

    [Tooltip("If true, analytics events will be recorded")]
    public bool optIn = false;

    [Tooltip("Number of events to buffer before flushing to disk")]
    public int bufferSize = 32;

    [Tooltip("Auto-flush interval in seconds (0 to disable)")]
    public float autoFlushInterval = 60f;

    private readonly List<string> _buffer = new List<string>();
    private string _dir;
    private StreamWriter _currentWriter;
    private string _currentLogDate;
    private float _lastFlushTime;
    private readonly object _bufferLock = new object();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("AnalyticsManager: Duplicate instance destroyed");
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _dir = Path.Combine(Application.persistentDataPath, "Telemetry");
        Directory.CreateDirectory(_dir);
        _lastFlushTime = Time.realtimeSinceStartup;
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            Flush();
            CloseWriter();
            _instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        Flush();
        CloseWriter();
    }

    private void Update()
    {
        // Auto-flush based on time interval
        if (autoFlushInterval > 0 && Time.realtimeSinceStartup - _lastFlushTime >= autoFlushInterval)
        {
            Flush();
            _lastFlushTime = Time.realtimeSinceStartup;
        }
    }

    public void Record(string eventName, Dictionary<string, object> fields = null)
    {
        if (!optIn) return;

        // Validate event name
        if (string.IsNullOrWhiteSpace(eventName))
        {
            Debug.LogWarning("AnalyticsManager: Event name cannot be null or empty");
            return;
        }

        // Sanitize event name (remove pipes and newlines)
        eventName = SanitizeString(eventName);

        var sb = new StringBuilder();
        sb.Append(DateTime.UtcNow.ToString("o"));
        sb.Append('|');
        sb.Append(eventName);

        if (fields != null)
        {
            foreach (var kv in fields)
            {
                if (string.IsNullOrWhiteSpace(kv.Key))
                {
                    continue;
                }

                var key = SanitizeString(kv.Key);
                var value = kv.Value == null ? "null" : SanitizeString(kv.Value.ToString());

                sb.Append('|');
                sb.Append(key);
                sb.Append('=');
                sb.Append(value);
            }
        }

        lock (_bufferLock)
        {
            _buffer.Add(sb.ToString());

            if (_buffer.Count >= bufferSize)
            {
                FlushInternal();
            }
        }
    }

    private string SanitizeString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // Remove characters that would break log parsing
        return input
            .Replace("|", "_")
            .Replace("\n", " ")
            .Replace("\r", " ")
            .Replace("=", "_");
    }

    public void Flush()
    {
        lock (_bufferLock)
        {
            FlushInternal();
        }
    }

    private void FlushInternal()
    {
        if (_buffer.Count == 0) return;

        try
        {
            var today = DateTime.UtcNow.ToString("yyyyMMdd");

            // Rotate log file if date changed
            if (_currentLogDate != today || _currentWriter == null)
            {
                CloseWriter();
                var path = Path.Combine(_dir, $"{today}.log");
                _currentWriter = new StreamWriter(path, append: true, Encoding.UTF8)
                {
                    AutoFlush = false
                };
                _currentLogDate = today;
            }

            foreach (var line in _buffer)
            {
                _currentWriter.WriteLine(line);
            }
            _currentWriter.Flush();
            _buffer.Clear();
        }
        catch (Exception ex)
        {
            Debug.LogError($"AnalyticsManager: Failed to flush buffer: {ex.Message}");
        }
    }

    private void CloseWriter()
    {
        if (_currentWriter != null)
        {
            try
            {
                _currentWriter.Dispose();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"AnalyticsManager: Error closing writer: {ex.Message}");
            }
            _currentWriter = null;
        }
    }

    public int BufferedEventCount
    {
        get
        {
            lock (_bufferLock)
            {
                return _buffer.Count;
            }
        }
    }
}
