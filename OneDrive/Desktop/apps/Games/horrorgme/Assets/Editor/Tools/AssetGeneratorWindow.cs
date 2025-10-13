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

    private void OnGUI()
    {
        EditorGUILayout.LabelField("External tooling paths", EditorStyles.boldLabel);
        pythonPath = EditorGUILayout.TextField("Python", pythonPath);
        nodePath = EditorGUILayout.TextField("Node", nodePath);
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
            var p = Process.Start(psi);
            string stdout = p.StandardOutput.ReadToEnd();
            string stderr = p.StandardError.ReadToEnd();
            p.WaitForExit();
            UnityEngine.Debug.Log($"{script} exited {p.ExitCode}\n{stdout}");
            if (!string.IsNullOrEmpty(stderr)) UnityEngine.Debug.LogWarning(stderr);
            AssetDatabase.Refresh();
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Asset Generator", "Failed: " + ex.Message + "\nEnsure required tools are installed.", "OK");
        }
    }
}


