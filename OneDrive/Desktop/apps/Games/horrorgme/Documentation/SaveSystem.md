# Save / Checkpoint System

Persistent player state stored as versioned JSON in `Application.persistentDataPath/saves/save_{slot}.json` with ScriptableObject defaults for new saves and migration.

## Contents
- sceneName (string)
- playerPosition (Vector3)
- choiceFlags (dictionary<string, int/bool/float/string>)
- timestampUtc (ISO8601)
- saveVersion (int)

## JSON schema (informal)
```json
{
  "saveVersion": 1,
  "timestampUtc": "2025-01-01T12:00:00.0000000Z",
  "sceneName": "Game",
  "playerPosition": { "x": 0, "y": 1, "z": 0 },
  "choiceFlags": {
    "demo": true,
    "difficulty": 2
  }
}
```

## Defaults
Create `Assets/ScriptableObjects/SaveDefaults.asset` via Create > Save > Defaults.
- defaultSceneName: initial scene when none present
- defaultPlayerPosition: fallback position

Optionally add a copy to `Resources/SaveDefaults` for runtime loading outside the Editor.

## API
- SaveManager.Save(slot, SaveData)
- SaveManager.TryLoad(slot, out SaveData)
- SaveManager.Migrate(SaveData) — simple forward migration
- SaveData.SetFlag(key, value) helpers and TryGetFlag* accessors

## Migration
If saveVersion < current, Migrate updates fields in place (extend per version).

## Tests
- Roundtrip_SerializeDeserialize ensures basic integrity
- Migration_Version0_To_Current verifies forward migration
