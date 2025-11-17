using UnityEditor;
using UnityEngine;
using System.IO;

public class BuildScript
{
    private static string[] GetScenes()
    {
        return new string[]
        {
            "Assets/Scenes/Game.unity"
        };
    }

    [MenuItem("Build/Build Windows x64")]
    public static void BuildWindows()
    {
        string buildPath = "Builds/Windows";
        string executableName = "HorrorGame.exe";

        // Ensure build directory exists
        Directory.CreateDirectory(buildPath);

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = Path.Combine(buildPath, executableName),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {report.summary.totalSize} bytes");
            Debug.Log($"Output: {buildOptions.locationPathName}");
        }
        else
        {
            Debug.LogError($"Build failed with {report.summary.totalErrors} errors");
        }
    }

    [MenuItem("Build/Build Linux x64")]
    public static void BuildLinux()
    {
        string buildPath = "Builds/Linux";
        string executableName = "HorrorGame";

        Directory.CreateDirectory(buildPath);

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = Path.Combine(buildPath, executableName),
            target = BuildTarget.StandaloneLinux64,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {report.summary.totalSize} bytes");
        }
        else
        {
            Debug.LogError($"Build failed with {report.summary.totalErrors} errors");
        }
    }

    [MenuItem("Build/Build macOS")]
    public static void BuildMacOS()
    {
        string buildPath = "Builds/macOS";
        string appName = "HorrorGame.app";

        Directory.CreateDirectory(buildPath);

        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = Path.Combine(buildPath, appName),
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(buildOptions);

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log($"Build succeeded: {report.summary.totalSize} bytes");
        }
        else
        {
            Debug.LogError($"Build failed with {report.summary.totalErrors} errors");
        }
    }

    // Command line build method
    public static void BuildFromCommandLine()
    {
        string targetPlatform = GetCommandLineArg("-buildTarget");

        switch (targetPlatform?.ToLower())
        {
            case "windows":
                BuildWindows();
                break;
            case "linux":
                BuildLinux();
                break;
            case "macos":
                BuildMacOS();
                break;
            default:
                BuildWindows(); // Default to Windows
                break;
        }
    }

    private static string GetCommandLineArg(string name)
    {
        string[] args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
