using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsManager", menuName = "Settings/Manager")]
public class SettingsManager : ScriptableObject
{
    [Range(0f, 1f)] public float masterVolume = 1f;
    public bool subtitlesOn = true;
    public GraphicsPreset graphicsPreset = GraphicsPreset.Medium;

    private static string SettingsFilePath => Path.Combine(Application.persistentDataPath, "settings.json");

    [Serializable]
    private class SettingsData
    {
        public float masterVolume = 1f;
        public bool subtitlesOn = true;
        public int graphicsPreset = 1;
    }

    /// <summary>
    /// Save current settings to disk as JSON
    /// </summary>
    public void SaveToDisk()
    {
        try
        {
            var data = new SettingsData
            {
                masterVolume = masterVolume,
                subtitlesOn = subtitlesOn,
                graphicsPreset = (int)graphicsPreset
            };

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);

            // Write to temp file first for atomic operation
            var tempPath = SettingsFilePath + ".tmp";
            File.WriteAllText(tempPath, json);

            if (File.Exists(SettingsFilePath))
            {
                File.Delete(SettingsFilePath);
            }
            File.Move(tempPath, SettingsFilePath);

            Debug.Log($"SettingsManager: Settings saved to {SettingsFilePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"SettingsManager: Failed to save settings: {ex.Message}");
        }
    }

    /// <summary>
    /// Load settings from disk
    /// </summary>
    public void LoadFromDisk()
    {
        if (!File.Exists(SettingsFilePath))
        {
            Debug.Log("SettingsManager: No saved settings found, using defaults");
            return;
        }

        try
        {
            var json = File.ReadAllText(SettingsFilePath);
            var data = JsonConvert.DeserializeObject<SettingsData>(json);

            if (data != null)
            {
                masterVolume = Mathf.Clamp01(data.masterVolume);
                subtitlesOn = data.subtitlesOn;

                if (Enum.IsDefined(typeof(GraphicsPreset), data.graphicsPreset))
                {
                    graphicsPreset = (GraphicsPreset)data.graphicsPreset;
                }
                else
                {
                    Debug.LogWarning($"SettingsManager: Invalid graphics preset {data.graphicsPreset}, using default");
                    graphicsPreset = GraphicsPreset.Medium;
                }

                Debug.Log($"SettingsManager: Settings loaded from {SettingsFilePath}");
            }
        }
        catch (JsonException ex)
        {
            Debug.LogError($"SettingsManager: Failed to parse settings JSON: {ex.Message}");
        }
        catch (IOException ex)
        {
            Debug.LogError($"SettingsManager: Failed to read settings file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"SettingsManager: Unexpected error loading settings: {ex.GetType().Name} - {ex.Message}");
        }
    }

    /// <summary>
    /// Reset all settings to defaults
    /// </summary>
    public void ResetToDefaults()
    {
        masterVolume = 1f;
        subtitlesOn = true;
        graphicsPreset = GraphicsPreset.Medium;
        Debug.Log("SettingsManager: Reset to default settings");
    }

    /// <summary>
    /// Delete saved settings file
    /// </summary>
    public bool DeleteSavedSettings()
    {
        try
        {
            if (File.Exists(SettingsFilePath))
            {
                File.Delete(SettingsFilePath);
                Debug.Log("SettingsManager: Deleted saved settings file");
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SettingsManager: Failed to delete settings file: {ex.Message}");
            return false;
        }
    }

    private void OnEnable()
    {
        // Automatically load settings when the ScriptableObject is enabled
        LoadFromDisk();
    }
}

public enum GraphicsPreset { Low, Medium, High }
