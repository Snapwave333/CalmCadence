using System.IO;
using NUnit.Framework;
using UnityEngine;

public class SettingsManagerTests
{
    private SettingsManager _settings;
    private string _settingsPath;

    [SetUp]
    public void SetUp()
    {
        _settings = ScriptableObject.CreateInstance<SettingsManager>();
        _settingsPath = Path.Combine(Application.persistentDataPath, "settings.json");

        // Clean up any existing settings file
        if (File.Exists(_settingsPath))
        {
            File.Delete(_settingsPath);
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (_settings != null)
        {
            Object.DestroyImmediate(_settings);
        }

        if (File.Exists(_settingsPath))
        {
            File.Delete(_settingsPath);
        }
    }

    [Test]
    public void DefaultValues_AreCorrect()
    {
        Assert.AreEqual(1f, _settings.masterVolume);
        Assert.IsTrue(_settings.subtitlesOn);
        Assert.AreEqual(GraphicsPreset.Medium, _settings.graphicsPreset);
    }

    [Test]
    public void SaveToDisk_CreatesFile()
    {
        _settings.SaveToDisk();

        Assert.IsTrue(File.Exists(_settingsPath));
    }

    [Test]
    public void SaveAndLoad_PreservesSettings()
    {
        _settings.masterVolume = 0.5f;
        _settings.subtitlesOn = false;
        _settings.graphicsPreset = GraphicsPreset.High;

        _settings.SaveToDisk();

        // Create new instance and load
        var newSettings = ScriptableObject.CreateInstance<SettingsManager>();
        newSettings.LoadFromDisk();

        Assert.AreEqual(0.5f, newSettings.masterVolume, 0.001f);
        Assert.IsFalse(newSettings.subtitlesOn);
        Assert.AreEqual(GraphicsPreset.High, newSettings.graphicsPreset);

        Object.DestroyImmediate(newSettings);
    }

    [Test]
    public void LoadFromDisk_WhenNoFile_UsesDefaults()
    {
        _settings.masterVolume = 0.3f;
        _settings.LoadFromDisk(); // No file exists

        // Should not change from what was set
        Assert.AreEqual(0.3f, _settings.masterVolume, 0.001f);
    }

    [Test]
    public void ResetToDefaults_RestoresDefaultValues()
    {
        _settings.masterVolume = 0.2f;
        _settings.subtitlesOn = false;
        _settings.graphicsPreset = GraphicsPreset.Low;

        _settings.ResetToDefaults();

        Assert.AreEqual(1f, _settings.masterVolume);
        Assert.IsTrue(_settings.subtitlesOn);
        Assert.AreEqual(GraphicsPreset.Medium, _settings.graphicsPreset);
    }

    [Test]
    public void DeleteSavedSettings_RemovesFile()
    {
        _settings.SaveToDisk();
        Assert.IsTrue(File.Exists(_settingsPath));

        var deleted = _settings.DeleteSavedSettings();

        Assert.IsTrue(deleted);
        Assert.IsFalse(File.Exists(_settingsPath));
    }

    [Test]
    public void DeleteSavedSettings_WhenNoFile_ReturnsFalse()
    {
        var deleted = _settings.DeleteSavedSettings();
        Assert.IsFalse(deleted);
    }

    [Test]
    public void LoadFromDisk_ClampsVolumeToValidRange()
    {
        // Manually write invalid JSON
        var json = @"{""masterVolume"": 1.5, ""subtitlesOn"": true, ""graphicsPreset"": 1}";
        File.WriteAllText(_settingsPath, json);

        _settings.LoadFromDisk();

        Assert.AreEqual(1f, _settings.masterVolume); // Should be clamped to 1
    }

    [Test]
    public void LoadFromDisk_HandlesInvalidGraphicsPreset()
    {
        var json = @"{""masterVolume"": 0.8, ""subtitlesOn"": true, ""graphicsPreset"": 99}";
        File.WriteAllText(_settingsPath, json);

        _settings.LoadFromDisk();

        Assert.AreEqual(GraphicsPreset.Medium, _settings.graphicsPreset); // Should default to Medium
    }

    [Test]
    public void LoadFromDisk_HandlesCorruptedJson()
    {
        File.WriteAllText(_settingsPath, "not valid json {");

        // Should not throw, just log error
        Assert.DoesNotThrow(() => _settings.LoadFromDisk());
    }
}
