using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Captures error logs and writes rolling files under persistentDataPath/Logs, and optionally uploads.
/// </summary>
public class LogUploader : MonoBehaviour
{
    [Tooltip("Opt-in: if true, logs will be queued for upload.")]
    public bool uploadOptIn = false;

    [Tooltip("Upload endpoint.")]
    public string uploadUrl = "http://localhost:5000/upload";

    private readonly Queue<string> _pending = new Queue<string>();
    private string _logsDir;

    private void Awake()
    {
        _logsDir = Path.Combine(Application.persistentDataPath, "Logs");
        Directory.CreateDirectory(_logsDir);
        Application.logMessageReceived += OnLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= OnLog;
    }

    private void OnLog(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            var entry = DateTime.UtcNow.ToString("o") + " | " + type + " | " + condition + "\n" + stackTrace + "\n";
            var path = Path.Combine(_logsDir, DateTime.UtcNow.ToString("yyyyMMdd")) + ".log";
            File.AppendAllText(path, entry, Encoding.UTF8);
            if (uploadOptIn)
            {
                _pending.Enqueue(entry);
                _ = TryUploadAsync(entry);
            }
        }
    }

    private async Task TryUploadAsync(string body)
    {
        try
        {
            var req = new UnityEngine.Networking.UnityWebRequest(uploadUrl, "POST");
            byte[] data = Encoding.UTF8.GetBytes(body);
            req.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(data);
            req.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "text/plain");
            await req.SendWebRequest();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Log upload failed: " + ex.Message);
        }
    }
}


