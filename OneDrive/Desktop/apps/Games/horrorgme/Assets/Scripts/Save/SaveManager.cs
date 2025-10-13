using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Manages reading and writing SaveData to disk as JSON.
/// </summary>
public static class SaveManager
{
    public const int CurrentSaveVersion = 1;
    private const string SavesFolderName = "saves";
    private const string FilePattern = "save_{0}.json";

    public static string GetSavesFolder()
    {
        var dir = Path.Combine(Application.persistentDataPath, SavesFolderName);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return dir;
    }

    public static string GetSavePath(int slot)
    {
        return Path.Combine(GetSavesFolder(), string.Format(FilePattern, slot));
    }

    public static void Save(int slot, SaveData data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        data.saveVersion = CurrentSaveVersion;
        data.timestampUtc = DateTime.UtcNow.ToString("o");
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(GetSavePath(slot), json);
    }

    public static bool TryLoad(int slot, out SaveData data)
    {
        var path = GetSavePath(slot);
        data = null;
        if (!File.Exists(path)) return false;
        var json = File.ReadAllText(path);
        data = JsonConvert.DeserializeObject<SaveData>(json);
        if (data == null) return false;
        if (data.saveVersion < CurrentSaveVersion)
        {
            Migrate(data);
        }
        return true;
    }

    /// <summary>
    /// Simple in-place migration logic for older save versions.
    /// Extend with explicit steps per version.
    /// </summary>
    public static void Migrate(SaveData data)
    {
        if (data == null) return;
        // Example: if version 0 -> 1 ensure timestamp and default scene
        if (data.saveVersion < 1)
        {
            if (string.IsNullOrEmpty(data.timestampUtc))
                data.timestampUtc = DateTime.UtcNow.ToString("o");
            if (string.IsNullOrEmpty(data.sceneName))
                data.sceneName = LoadDefaults()?.defaultSceneName ?? "Game";
            data.saveVersion = 1;
        }
    }

    public static SaveDefaults LoadDefaults()
    {
#if UNITY_EDITOR
        // Editor-only: attempt to load directly from AssetDatabase for convenience
        var asset = (SaveDefaults)UnityEditor.AssetDatabase.LoadAssetAtPath(
            "Assets/ScriptableObjects/SaveDefaults.asset", typeof(SaveDefaults));
        if (asset != null) return asset;
#endif
        // Runtime: attempt Resources
        return Resources.Load<SaveDefaults>("SaveDefaults");
    }
}


