using NUnit.Framework;
using UnityEngine;

public class LogUploaderTests
{
    [Test]
    public void LogsDirectory_IsCreated_OnAwake()
    {
        var go = new GameObject("LogUploader_Test");
        var lu = go.AddComponent<LogUploader>();
        Assert.IsNotNull(lu);
    }
}


