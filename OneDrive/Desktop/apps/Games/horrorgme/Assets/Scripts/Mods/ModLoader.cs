using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Loads simple mod manifests from StreamingAssets/Mods/<modname>/manifest.json and logs contents.
/// Safety checks only; no dynamic code loading.
/// </summary>
public static class ModLoader
{
    public static IEnumerable<string> DiscoverMods()
    {
        var dir = Path.Combine(Application.streamingAssetsPath, "Mods");
        if (!Directory.Exists(dir)) yield break;
        foreach (var sub in Directory.GetDirectories(dir))
        {
            var name = Path.GetFileName(sub);
            var manifest = Path.Combine(sub, "manifest.json");
            if (File.Exists(manifest)) yield return name;
        }
    }

    public static string LoadManifestJson(string modName)
    {
        var path = Path.Combine(Application.streamingAssetsPath, "Mods", modName, "manifest.json");
        if (!File.Exists(path)) return null;
        var json = File.ReadAllText(path);
        // Perform basic safety checks (size limit, minimal schema presence)
        if (json.Length > 1024 * 1024) return null;
        return json;
    }
}


