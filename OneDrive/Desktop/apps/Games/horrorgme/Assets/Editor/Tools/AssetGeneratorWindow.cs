using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor window to run local Python/Node asset generation scripts and refresh the AssetDatabase.
/// </summary>
public class AssetGeneratorWindow : EditorWindow
{
    [MenuItem("Tools/Assets/Asset Generator")] public static void Open() => GetWindow<AssetGeneratorWindow>(true, "Asset Generator");

    private string pythonPath = "python";
    private string nodePath = "node";
    private int timeoutMs = 60000; // 60 second timeout

    private void OnGUI()
    {
        EditorGUILayout.LabelField("External tooling paths", EditorStyles.boldLabel);
        pythonPath = EditorGUILayout.TextField("Python", pythonPath);
        nodePath = EditorGUILayout.TextField("Node", nodePath);
        timeoutMs = EditorGUILayout.IntField("Timeout (ms)", timeoutMs);
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Textures (Python)"))
            RunProcess(pythonPath, "generate_assets.py");

        if (GUILayout.Button("Generate Audio (Python)"))
            RunProcess(pythonPath, "generate_audio.py");

        if (GUILayout.Button("Generate Meshes (Python)"))
            RunProcess(pythonPath, "generate_meshes.py");

        if (GUILayout.Button("Generate Assets (Node)"))
            RunProcess(nodePath, "generate_assets.js");

        EditorGUILayout.Space();
        if (GUILayout.Button("Run All"))
        {
            RunProcess(pythonPath, "generate_assets.py");
            RunProcess(pythonPath, "generate_audio.py");
            RunProcess(pythonPath, "generate_meshes.py");
            RunProcess(nodePath, "generate_assets.js");
        }
    }

    private void RunProcess(string executable, string script)
    {
        Process process = null;
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = executable,
                Arguments = script,
                WorkingDirectory = Application.dataPath + "/..",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            process = Process.Start(psi);
            if (process == null)
            {
                EditorUtility.DisplayDialog("Asset Generator", $"Failed to start process: {executable}", "OK");
                return;
            }

            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            bool exited = process.WaitForExit(timeoutMs);
            if (!exited)
            {
                process.Kill();
                EditorUtility.DisplayDialog("Asset Generator", $"Process timed out after {timeoutMs}ms: {script}", "OK");
                return;
            }

            int exitCode = process.ExitCode;
            UnityEngine.Debug.Log($"{script} exited {exitCode}\n{stdout}");

            if (!string.IsNullOrEmpty(stderr))
            {
                UnityEngine.Debug.LogWarning($"{script} stderr:\n{stderr}");
            }

            if (exitCode != 0)
            {
                EditorUtility.DisplayDialog("Asset Generator", $"{script} failed with exit code {exitCode}.\nCheck console for details.", "OK");
            }

            AssetDatabase.Refresh();
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            EditorUtility.DisplayDialog("Asset Generator", $"Failed to run '{executable}':\n{ex.Message}\n\nEnsure the executable is installed and in PATH.", "OK");
        }
        catch (InvalidOperationException ex)
        {
            EditorUtility.DisplayDialog("Asset Generator", $"Process error: {ex.Message}", "OK");
        }
        catch (Exception ex)
        {
            EditorUtility.DisplayDialog("Asset Generator", $"Unexpected error: {ex.GetType().Name}\n{ex.Message}", "OK");
        }
        finally
        {
            process?.Dispose();
        }
    }
}
