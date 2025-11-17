using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Serializable save data payload.
/// </summary>
[Serializable]
public class SaveData
{
    [JsonProperty("saveVersion")] public int saveVersion;
    [JsonProperty("timestampUtc")] public string timestampUtc;

    // Scene
    [JsonProperty("sceneName")] public string sceneName;

    // Player
    [JsonProperty("playerPosition")] public Vector3 playerPosition;

    // Choice flags
    [JsonProperty("choiceFlags")] public Dictionary<string, JToken> choiceFlags = new Dictionary<string, JToken>(StringComparer.OrdinalIgnoreCase);

    public SaveData() { }

    public SaveData(int version)
    {
        saveVersion = version;
        timestampUtc = DateTime.UtcNow.ToString("o");
    }

    public void SetFlag(string key, bool value) => choiceFlags[key] = new JValue(value);
    public void SetFlag(string key, int value) => choiceFlags[key] = new JValue(value);
    public void SetFlag(string key, float value) => choiceFlags[key] = new JValue(value);
    public void SetFlag(string key, string value) => choiceFlags[key] = new JValue(value);

    public bool TryGetFlagBool(string key, out bool value)
    {
        value = false;
        if (!choiceFlags.TryGetValue(key, out var token)) return false;
        if (token == null) return false;
        try
        {
            value = token.Value<bool>();
            return true;
        }
        catch (FormatException ex)
        {
            Debug.LogWarning($"SaveData: Failed to parse flag '{key}' as bool: {ex.Message}");
            return false;
        }
        catch (InvalidCastException ex)
        {
            Debug.LogWarning($"SaveData: Flag '{key}' cannot be cast to bool: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveData: Unexpected error parsing flag '{key}' as bool: {ex.GetType().Name} - {ex.Message}");
            return false;
        }
    }

    public bool TryGetFlagInt(string key, out int value)
    {
        value = 0;
        if (!choiceFlags.TryGetValue(key, out var token)) return false;
        if (token == null) return false;
        try
        {
            value = token.Value<int>();
            return true;
        }
        catch (FormatException ex)
        {
            Debug.LogWarning($"SaveData: Failed to parse flag '{key}' as int: {ex.Message}");
            return false;
        }
        catch (InvalidCastException ex)
        {
            Debug.LogWarning($"SaveData: Flag '{key}' cannot be cast to int: {ex.Message}");
            return false;
        }
        catch (OverflowException ex)
        {
            Debug.LogWarning($"SaveData: Flag '{key}' value overflows int: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveData: Unexpected error parsing flag '{key}' as int: {ex.GetType().Name} - {ex.Message}");
            return false;
        }
    }

    public bool TryGetFlagFloat(string key, out float value)
    {
        value = 0f;
        if (!choiceFlags.TryGetValue(key, out var token)) return false;
        if (token == null) return false;
        try
        {
            value = token.Value<float>();
            return true;
        }
        catch (FormatException ex)
        {
            Debug.LogWarning($"SaveData: Failed to parse flag '{key}' as float: {ex.Message}");
            return false;
        }
        catch (InvalidCastException ex)
        {
            Debug.LogWarning($"SaveData: Flag '{key}' cannot be cast to float: {ex.Message}");
            return false;
        }
        catch (OverflowException ex)
        {
            Debug.LogWarning($"SaveData: Flag '{key}' value overflows float: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveData: Unexpected error parsing flag '{key}' as float: {ex.GetType().Name} - {ex.Message}");
            return false;
        }
    }

    public bool TryGetFlagString(string key, out string value)
    {
        value = null;
        if (!choiceFlags.TryGetValue(key, out var token)) return false;
        if (token == null) return false;
        try
        {
            value = token.Value<string>();
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveData: Unexpected error parsing flag '{key}' as string: {ex.GetType().Name} - {ex.Message}");
            return false;
        }
    }
}
