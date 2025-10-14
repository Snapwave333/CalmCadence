using System.IO;
using UnityEngine;

public class ScreenshotTool : MonoBehaviour
{
    public int superSize = 2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            var dir = Path.Combine(Application.dataPath, "..", "Documentation", "media", "screenshots");
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, "shot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
            var prev = GUI.enabled;
            GUI.enabled = false; // attempt to hide UI
            ScreenCapture.CaptureScreenshot(path, superSize);
            GUI.enabled = prev;
        }
    }
}


