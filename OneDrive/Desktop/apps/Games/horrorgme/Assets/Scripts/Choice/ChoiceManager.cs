using System;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    private static ChoiceManager _instance;
    public static ChoiceManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("ChoiceManager");
                _instance = go.AddComponent<ChoiceManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private readonly Dictionary<string, object> _flags = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Action<object>> _subscriptions = new Dictionary<string, Action<object>>(StringComparer.OrdinalIgnoreCase);

    public void SetFlag(string key, object value)
    {
        _flags[key] = value;
        if (_subscriptions.TryGetValue(key, out var cb) && cb != null)
        {
            cb(value);
        }
    }

    public T GetFlag<T>(string key, T defaultValue = default(T))
    {
        if (_flags.TryGetValue(key, out var v) && v is T t) return t;
        return defaultValue;
    }

    public void SubscribeToFlagChanges(string key, Action<object> callback)
    {
        if (_subscriptions.ContainsKey(key)) _subscriptions[key] += callback; else _subscriptions[key] = callback;
    }
}


