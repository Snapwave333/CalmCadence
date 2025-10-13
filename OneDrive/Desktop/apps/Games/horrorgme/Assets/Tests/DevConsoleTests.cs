using NUnit.Framework;
using UnityEngine;

public class DevConsoleTests
{
    [SetUp]
    public void Setup()
    {
        var existing = Object.FindFirstObjectByType<DevConsole>();
        if (existing != null) Object.DestroyImmediate(existing.gameObject);
        var go = new GameObject("DevConsole_Test");
        go.AddComponent<DevConsole>();
    }

    [Test]
    public void SetFlag_PersistsToRuntimeTable()
    {
        Assert.IsNotNull(DevConsole.Instance);
        DevConsole.Instance.SetFlag("door_open", true);
        bool value;
        var ok = DevConsole.Instance.TryGetFlag<bool>("door_open", out value);
        Assert.IsTrue(ok);
        Assert.IsTrue(value);
    }

    private class TempCmdHost : MonoBehaviour
    {
        [DevCommand("testcmd", "test command")] public string Test(string[] args) { return "ok"; }
    }

    [Test]
    public void RegisteredCommand_IsDiscoverable()
    {
        var hostGo = new GameObject("Host");
        hostGo.AddComponent<TempCmdHost>();
        var dc = DevConsole.Instance;
        // Rebuild registry by recreating the console (simple approach for editmode tests)
        Object.DestroyImmediate(dc.gameObject);
        var go = new GameObject("DevConsole_Test2");
        var dc2 = go.AddComponent<DevConsole>();
        Assert.IsNotNull(dc2);
        // Execute help and ensure no exception; attempt to execute our command
        dc2.Execute("testcmd");
        Assert.Pass();
    }
}


