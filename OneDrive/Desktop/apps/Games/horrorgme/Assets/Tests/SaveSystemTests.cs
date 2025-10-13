using NUnit.Framework;
using System.IO;
using UnityEngine;

public class SaveSystemTests
{
    private string _tempDir;

    [SetUp]
    public void Setup()
    {
        _tempDir = Path.Combine(Application.temporaryCachePath, "SaveTests_" + System.Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    [TearDown]
    public void Teardown()
    {
        if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, true);
    }

    [Test]
    public void Roundtrip_SerializeDeserialize()
    {
        var data = new SaveData(SaveManager.CurrentSaveVersion)
        {
            sceneName = "Game",
            playerPosition = new Vector3(1, 2, 3)
        };
        data.SetFlag("demo", true);
        // Save to temp location
        var path = Path.Combine(_tempDir, "save_1.json");
        File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(data));

        var json = File.ReadAllText(path);
        var loaded = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(json);
        Assert.IsNotNull(loaded);
        Assert.AreEqual(new Vector3(1,2,3), loaded.playerPosition);
        bool flag;
        Assert.IsTrue(loaded.TryGetFlagBool("demo", out flag));
        Assert.IsTrue(flag);
    }

    [Test]
    public void Migration_Version0_To_Current()
    {
        var legacyJson = "{\n  \"saveVersion\": 0,\n  \"sceneName\": \"\",\n  \"playerPosition\": { \"x\": 0, \"y\": 0, \"z\": 0 }\n}";
        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveData>(legacyJson);
        Assert.IsNotNull(data);
        SaveManager.Migrate(data);
        Assert.GreaterOrEqual(data.saveVersion, SaveManager.CurrentSaveVersion);
        Assert.IsFalse(string.IsNullOrEmpty(data.timestampUtc));
        Assert.IsFalse(string.IsNullOrEmpty(data.sceneName));
    }
}


