using UnityEngine;

/// <summary>
/// Ensures the developer overlay exists in Editor/Development builds.
/// Tries to find a prefab named "DevOverlay" under Resources or in scene.
/// </summary>
public static class DevBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureDevOverlay()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Object.FindFirstObjectByType<DevConsole>() != null) return;

        // Try to load from Resources/Dev/DevOverlay
        var overlayPrefab = Resources.Load<GameObject>("Dev/DevOverlay");
        if (overlayPrefab != null)
        {
            var inst = Object.Instantiate(overlayPrefab);
            if (inst.GetComponentInChildren<DevConsole>() == null)
            {
                inst.AddComponent<DevConsole>();
            }
            return;
        }

        // Fallback: create a minimal GameObject so tests can run without prefab present
        var go = new GameObject("__DevConsole__");
        go.AddComponent<DevConsole>();
#endif
    }
}


