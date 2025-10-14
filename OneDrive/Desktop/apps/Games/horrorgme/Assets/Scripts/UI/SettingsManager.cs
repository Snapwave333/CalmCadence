using UnityEngine;

[CreateAssetMenu(fileName = "SettingsManager", menuName = "Settings/Manager")]
public class SettingsManager : ScriptableObject
{
    [Range(0f, 1f)] public float masterVolume = 1f;
    public bool subtitlesOn = true;
    public GraphicsPreset graphicsPreset = GraphicsPreset.Medium;
}

public enum GraphicsPreset { Low, Medium, High }


