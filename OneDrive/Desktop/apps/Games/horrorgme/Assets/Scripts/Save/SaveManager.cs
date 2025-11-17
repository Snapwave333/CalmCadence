using System;
using System.IO;
using System.Text.RegularExpressions;
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
    private const int MaxSlot = 999;
    private const int MinSlot = 0;

    // Regex to validate slot produces a safe filename
    private static readonly Regex SafeSlotPattern = new Regex(@"^save_\d+\.json$", RegexOptions.Compiled);

    public static string GetSavesFolder()
    {
        var dir = Path.Combine(Application.persistentDataPath, SavesFolderName);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return dir;
    }

    public static string GetSavePath(int slot)
    {
        ValidateSlot(slot);
        var filename = string.Format(FilePattern, slot);

        // Double-check the filename is safe
        if (!SafeSlotPattern.IsMatch(filename))
        {
            throw new ArgumentException($"Invalid slot {slot} produced unsafe filename: {filename}");
        }

        var path = Path.Combine(GetSavesFolder(), filename);

        // Ensure the path is within the saves folder (prevent directory traversal)
        var fullPath = Path.GetFullPath(path);
        var savesFolder = Path.GetFullPath(GetSavesFolder());
        if (!fullPath.StartsWith(savesFolder))
        {
            throw new ArgumentException($"Path traversal detected for slot {slot}");
        }

        return path;
    }

    private static void ValidateSlot(int slot)
    {
        if (slot < MinSlot || slot > MaxSlot)
        {
            throw new ArgumentOutOfRangeException(nameof(slot), $"Slot must be between {MinSlot} and {MaxSlot}, got {slot}");
        }
    }

    public static void Save(int slot, SaveData data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        ValidateSlot(slot);

        data.saveVersion = CurrentSaveVersion;
        data.timestampUtc = DateTime.UtcNow.ToString("o");

        var path = GetSavePath(slot);
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);

        // Write to temp file first, then move (atomic operation)
        var tempPath = path + ".tmp";
        try
        {
            File.WriteAllText(tempPath, json);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.Move(tempPath, path);
        }
        catch (Exception ex)
        {
            // Clean up temp file on failure
            try { File.Delete(tempPath); } catch { }
            Debug.LogError($"SaveManager: Failed to save slot {slot}: {ex.Message}");
            throw;
        }
    }

    public static bool TryLoad(int slot, out SaveData data)
    {
        data = null;

        try
        {
            ValidateSlot(slot);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Debug.LogWarning($"SaveManager: {ex.Message}");
            return false;
        }

        var path = GetSavePath(slot);
        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogWarning($"SaveManager: Save file at slot {slot} is empty");
                return false;
            }

            data = JsonConvert.DeserializeObject<SaveData>(json);
            if (data == null)
            {
                Debug.LogWarning($"SaveManager: Failed to deserialize save data at slot {slot}");
                return false;
            }

            if (data.saveVersion < CurrentSaveVersion)
            {
                Migrate(data);
            }

            return true;
        }
        catch (JsonException ex)
        {
            Debug.LogError($"SaveManager: JSON parsing error for slot {slot}: {ex.Message}");
            return false;
        }
        catch (IOException ex)
        {
            Debug.LogError($"SaveManager: IO error reading slot {slot}: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveManager: Unexpected error loading slot {slot}: {ex.GetType().Name} - {ex.Message}");
            return false;
        }
    }

    public static bool SaveExists(int slot)
    {
        try
        {
            ValidateSlot(slot);
            return File.Exists(GetSavePath(slot));
        }
        catch
        {
            return false;
        }
    }

    public static bool DeleteSave(int slot)
    {
        try
        {
            ValidateSlot(slot);
            var path = GetSavePath(slot);
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveManager: Failed to delete slot {slot}: {ex.Message}");
            return false;
        }
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

        // Add future migrations here:
        // if (data.saveVersion < 2) { ... data.saveVersion = 2; }
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
