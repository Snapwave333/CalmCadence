using System;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class SaveManagerTests
{
    private string _originalPath;
    private string _testPath;

    [SetUp]
    public void SetUp()
    {
        _testPath = Path.Combine(Application.temporaryCachePath, "TestSaves");
        if (Directory.Exists(_testPath))
        {
            Directory.Delete(_testPath, true);
        }
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testPath))
        {
            Directory.Delete(_testPath, true);
        }
    }

    [Test]
    public void ValidateSlot_ThrowsOnNegativeSlot()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SaveManager.GetSavePath(-1));
    }

    [Test]
    public void ValidateSlot_ThrowsOnTooLargeSlot()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => SaveManager.GetSavePath(1000));
    }

    [Test]
    public void ValidateSlot_AcceptsValidSlot()
    {
        Assert.DoesNotThrow(() => SaveManager.GetSavePath(1));
        Assert.DoesNotThrow(() => SaveManager.GetSavePath(0));
        Assert.DoesNotThrow(() => SaveManager.GetSavePath(999));
    }

    [Test]
    public void GetSavePath_ReturnsCorrectFormat()
    {
        var path = SaveManager.GetSavePath(1);
        Assert.IsTrue(path.EndsWith("save_1.json"));
    }

    [Test]
    public void Save_ThrowsOnNullData()
    {
        Assert.Throws<ArgumentNullException>(() => SaveManager.Save(1, null));
    }

    [Test]
    public void Save_SetsVersionAndTimestamp()
    {
        var data = new SaveData();
        data.sceneName = "TestScene";

        SaveManager.Save(1, data);

        Assert.AreEqual(SaveManager.CurrentSaveVersion, data.saveVersion);
        Assert.IsFalse(string.IsNullOrEmpty(data.timestampUtc));
    }

    [Test]
    public void TryLoad_ReturnsFalseForNonexistentFile()
    {
        var result = SaveManager.TryLoad(999, out var data);

        Assert.IsFalse(result);
        Assert.IsNull(data);
    }

    [Test]
    public void SaveAndLoad_RoundTripsData()
    {
        var original = new SaveData(SaveManager.CurrentSaveVersion);
        original.sceneName = "TestScene";
        original.playerPosition = new Vector3(1, 2, 3);
        original.SetFlag("testBool", true);
        original.SetFlag("testInt", 42);
        original.SetFlag("testFloat", 3.14f);

        SaveManager.Save(1, original);
        var loaded = SaveManager.TryLoad(1, out var data);

        Assert.IsTrue(loaded);
        Assert.AreEqual(original.sceneName, data.sceneName);
        Assert.AreEqual(original.playerPosition, data.playerPosition);

        Assert.IsTrue(data.TryGetFlagBool("testBool", out var boolVal));
        Assert.IsTrue(boolVal);

        Assert.IsTrue(data.TryGetFlagInt("testInt", out var intVal));
        Assert.AreEqual(42, intVal);

        Assert.IsTrue(data.TryGetFlagFloat("testFloat", out var floatVal));
        Assert.AreEqual(3.14f, floatVal, 0.001f);
    }

    [Test]
    public void SaveExists_ReturnsTrueForExistingSave()
    {
        var data = new SaveData(SaveManager.CurrentSaveVersion);
        data.sceneName = "Test";
        SaveManager.Save(1, data);

        Assert.IsTrue(SaveManager.SaveExists(1));
    }

    [Test]
    public void SaveExists_ReturnsFalseForNonexistentSave()
    {
        Assert.IsFalse(SaveManager.SaveExists(999));
    }

    [Test]
    public void DeleteSave_RemovesSaveFile()
    {
        var data = new SaveData(SaveManager.CurrentSaveVersion);
        data.sceneName = "Test";
        SaveManager.Save(1, data);

        Assert.IsTrue(SaveManager.SaveExists(1));

        var deleted = SaveManager.DeleteSave(1);

        Assert.IsTrue(deleted);
        Assert.IsFalse(SaveManager.SaveExists(1));
    }

    [Test]
    public void DeleteSave_ReturnsFalseForNonexistentSave()
    {
        var deleted = SaveManager.DeleteSave(999);
        Assert.IsFalse(deleted);
    }

    [Test]
    public void Migrate_UpdatesOldSaveVersion()
    {
        var data = new SaveData();
        data.saveVersion = 0;
        data.sceneName = "";
        data.timestampUtc = "";

        SaveManager.Migrate(data);

        Assert.AreEqual(SaveManager.CurrentSaveVersion, data.saveVersion);
        Assert.IsFalse(string.IsNullOrEmpty(data.timestampUtc));
    }

    [Test]
    public void SaveData_TryGetFlag_ReturnsFalseForMissingKey()
    {
        var data = new SaveData();

        Assert.IsFalse(data.TryGetFlagBool("missing", out _));
        Assert.IsFalse(data.TryGetFlagInt("missing", out _));
        Assert.IsFalse(data.TryGetFlagFloat("missing", out _));
        Assert.IsFalse(data.TryGetFlagString("missing", out _));
    }

    [Test]
    public void SaveData_SetAndGetFlag_WorksForAllTypes()
    {
        var data = new SaveData();

        data.SetFlag("bool", true);
        data.SetFlag("int", 100);
        data.SetFlag("float", 2.5f);
        data.SetFlag("string", "hello");

        Assert.IsTrue(data.TryGetFlagBool("bool", out var b) && b);
        Assert.IsTrue(data.TryGetFlagInt("int", out var i) && i == 100);
        Assert.IsTrue(data.TryGetFlagFloat("float", out var f) && Mathf.Abs(f - 2.5f) < 0.001f);
        Assert.IsTrue(data.TryGetFlagString("string", out var s) && s == "hello");
    }
}
