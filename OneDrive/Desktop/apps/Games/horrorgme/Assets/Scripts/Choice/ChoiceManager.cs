using System;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    private static ChoiceManager _instance;
    private static readonly object _lock = new object();
    private static bool _applicationQuitting = false;

    public static ChoiceManager Instance
    {
        get
        {
            if (_applicationQuitting)
            {
                Debug.LogWarning("ChoiceManager: Instance requested after application quit");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // First check if one exists in the scene
                    _instance = FindFirstObjectByType<ChoiceManager>();

                    if (_instance == null)
                    {
                        var go = new GameObject("ChoiceManager");
                        _instance = go.AddComponent<ChoiceManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
    }

    private readonly Dictionary<string, object> _flags = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, List<Action<object>>> _subscriptions = new Dictionary<string, List<Action<object>>>(StringComparer.OrdinalIgnoreCase);

    private void Awake()
    {
        lock (_lock)
        {
            if (_instance != null && _instance != this)
            {
                Debug.LogWarning("ChoiceManager: Duplicate instance destroyed");
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        lock (_lock)
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }

    private void OnApplicationQuit()
    {
        _applicationQuitting = true;
    }

    public void SetFlag(string key, object value)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("ChoiceManager: Cannot set flag with null or empty key");
            return;
        }

        _flags[key] = value;

        if (_subscriptions.TryGetValue(key, out var callbacks) && callbacks != null)
        {
            // Create a copy to allow modifications during iteration
            var callbacksCopy = new List<Action<object>>(callbacks);
            foreach (var cb in callbacksCopy)
            {
                try
                {
                    cb?.Invoke(value);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"ChoiceManager: Error in flag callback for '{key}': {ex.Message}");
                }
            }
        }
    }

    public T GetFlag<T>(string key, T defaultValue = default(T))
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("ChoiceManager: Cannot get flag with null or empty key");
            return defaultValue;
        }

        if (_flags.TryGetValue(key, out var v))
        {
            if (v is T t)
            {
                return t;
            }

            // Try to convert if types are compatible
            try
            {
                if (v != null && typeof(T).IsAssignableFrom(v.GetType()))
                {
                    return (T)v;
                }

                // Handle numeric conversions
                if (typeof(T) == typeof(float) && v is int intVal)
                {
                    return (T)(object)(float)intVal;
                }
                if (typeof(T) == typeof(int) && v is float floatVal)
                {
                    return (T)(object)(int)floatVal;
                }
                if (typeof(T) == typeof(string) && v != null)
                {
                    return (T)(object)v.ToString();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"ChoiceManager: Failed to convert flag '{key}' from {v?.GetType()?.Name ?? "null"} to {typeof(T).Name}: {ex.Message}");
            }
        }

        return defaultValue;
    }

    public bool HasFlag(string key)
    {
        return !string.IsNullOrEmpty(key) && _flags.ContainsKey(key);
    }

    public bool RemoveFlag(string key)
    {
        if (string.IsNullOrEmpty(key))
            return false;
        return _flags.Remove(key);
    }

    public void ClearAllFlags()
    {
        _flags.Clear();
    }

    public IReadOnlyDictionary<string, object> GetAllFlags()
    {
        return _flags;
    }

    public void SubscribeToFlagChanges(string key, Action<object> callback)
    {
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("ChoiceManager: Cannot subscribe with null or empty key");
            return;
        }
        if (callback == null)
        {
            Debug.LogWarning("ChoiceManager: Cannot subscribe with null callback");
            return;
        }

        if (!_subscriptions.TryGetValue(key, out var list))
        {
            list = new List<Action<object>>();
            _subscriptions[key] = list;
        }
        list.Add(callback);
    }

    public void UnsubscribeFromFlagChanges(string key, Action<object> callback)
    {
        if (string.IsNullOrEmpty(key) || callback == null)
            return;

        if (_subscriptions.TryGetValue(key, out var list))
        {
            list.Remove(callback);
        }
    }

    public void ClearSubscriptions(string key)
    {
        if (!string.IsNullOrEmpty(key))
        {
            _subscriptions.Remove(key);
        }
    }
}
