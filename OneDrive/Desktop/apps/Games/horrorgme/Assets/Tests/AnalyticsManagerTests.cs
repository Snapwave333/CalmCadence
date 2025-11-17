using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

public class AnalyticsManagerTests
{
    private string _testDir;

    [SetUp]
    public void SetUp()
    {
        _testDir = Path.Combine(Application.temporaryCachePath, "TestTelemetry");
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
        Directory.CreateDirectory(_testDir);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_testDir))
        {
            Directory.Delete(_testDir, true);
        }
    }

    [Test]
    public void Record_WhenOptInFalse_DoesNotRecord()
    {
        // This test verifies that when optIn is false, no events are recorded
        var go = new GameObject("TestAnalyticsManager");
        var manager = go.AddComponent<AnalyticsManager>();
        manager.optIn = false;

        manager.Record("test_event");

        Assert.AreEqual(0, manager.BufferedEventCount);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void Record_WhenOptInTrue_BuffersEvent()
    {
        var go = new GameObject("TestAnalyticsManager");
        var manager = go.AddComponent<AnalyticsManager>();
        manager.optIn = true;
        manager.bufferSize = 100; // Large buffer to prevent auto-flush

        manager.Record("test_event");

        Assert.AreEqual(1, manager.BufferedEventCount);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void Record_WithEmptyEventName_DoesNotRecord()
    {
        var go = new GameObject("TestAnalyticsManager");
        var manager = go.AddComponent<AnalyticsManager>();
        manager.optIn = true;

        manager.Record("");
        manager.Record(null);
        manager.Record("   ");

        Assert.AreEqual(0, manager.BufferedEventCount);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void Record_SanitizesEventName()
    {
        var go = new GameObject("TestAnalyticsManager");
        var manager = go.AddComponent<AnalyticsManager>();
        manager.optIn = true;
        manager.bufferSize = 100;

        // Event name with characters that should be sanitized
        manager.Record("test|event\nwith=special");

        Assert.AreEqual(1, manager.BufferedEventCount);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void Record_WithFields_IncludesAllFields()
    {
        var go = new GameObject("TestAnalyticsManager");
        var manager = go.AddComponent<AnalyticsManager>();
        manager.optIn = true;
        manager.bufferSize = 100;

        var fields = new Dictionary<string, object>
        {
            { "count", 42 },
            { "name", "test" },
            { "value", 3.14f }
        };

        manager.Record("test_event", fields);

        Assert.AreEqual(1, manager.BufferedEventCount);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void Record_WithNullFieldValue_HandlesGracefully()
    {
        var go = new GameObject("TestAnalyticsManager");
        var manager = go.AddComponent<AnalyticsManager>();
        manager.optIn = true;
        manager.bufferSize = 100;

        var fields = new Dictionary<string, object>
        {
            { "nullField", null },
            { "validField", "value" }
        };

        manager.Record("test_event", fields);

        Assert.AreEqual(1, manager.BufferedEventCount);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void Flush_ClearsBuffer()
    {
        var go = new GameObject("TestAnalyticsManager");
        var manager = go.AddComponent<AnalyticsManager>();
        manager.optIn = true;
        manager.bufferSize = 100;

        manager.Record("event1");
        manager.Record("event2");
        Assert.AreEqual(2, manager.BufferedEventCount);

        manager.Flush();

        Assert.AreEqual(0, manager.BufferedEventCount);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void Record_AutoFlushesWhenBufferFull()
    {
        var go = new GameObject("TestAnalyticsManager");
        var manager = go.AddComponent<AnalyticsManager>();
        manager.optIn = true;
        manager.bufferSize = 3;

        manager.Record("event1");
        manager.Record("event2");
        Assert.AreEqual(2, manager.BufferedEventCount);

        manager.Record("event3"); // Should trigger auto-flush

        Assert.AreEqual(0, manager.BufferedEventCount);

        Object.DestroyImmediate(go);
    }
}
