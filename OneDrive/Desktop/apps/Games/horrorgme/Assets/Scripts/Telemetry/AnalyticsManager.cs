using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public bool optIn = false;
    private readonly List<string> _buffer = new List<string>();
    private string _dir;

    private void Awake()
    {
        _dir = Path.Combine(Application.persistentDataPath, "Telemetry");
        Directory.CreateDirectory(_dir);
    }

    public void Record(string eventName, Dictionary<string, object> fields = null)
    {
        if (!optIn) return;
        var line = System.DateTime.UtcNow.ToString("o") + "|" + eventName;
        if (fields != null)
        {
            foreach (var kv in fields)
            {
                line += "|" + kv.Key + "=" + (kv.Value == null ? "null" : kv.Value.ToString());
            }
        }
        _buffer.Add(line);
        if (_buffer.Count >= 32) Flush();
    }

    public void Flush()
    {
        if (_buffer.Count == 0) return;
        var path = Path.Combine(_dir, System.DateTime.UtcNow.ToString("yyyyMMdd")) + ".log";
        File.AppendAllLines(path, _buffer, Encoding.UTF8);
        _buffer.Clear();
    }
}


