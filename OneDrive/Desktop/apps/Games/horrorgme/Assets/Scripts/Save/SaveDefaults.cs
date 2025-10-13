using UnityEngine;

/// <summary>
/// Default values for new saves and migration fallbacks.
/// Create via Assets > Create > Save > Defaults and place at Assets/ScriptableObjects/SaveDefaults.asset.
/// Optionally also add a copy to Resources with name "SaveDefaults" for runtime loading.
/// </summary>
[CreateAssetMenu(fileName = "SaveDefaults", menuName = "Save/Defaults")]
public class SaveDefaults : ScriptableObject
{
    public string defaultSceneName = "Game";
    public Vector3 defaultPlayerPosition = new Vector3(0, 1, 0);
}


