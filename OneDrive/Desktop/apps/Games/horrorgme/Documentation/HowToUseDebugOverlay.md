# Developer Debug Overlay / In-Game Console

This overlay provides fast, in-game debugging and command execution during development builds.

## Toggle
- Press `~` (BackQuote) to show/hide the overlay.
- In Editor/Development builds, a small on-screen toggle button is available.

## Display
- Current scene name and time
- Choice flags table (key → value)
- Searchable list of registered commands
- Console output and input field

## Built-in commands
- `help` — list registered commands
- `trigger <eventId>` — stub event trigger (logs only)
- `spawn <prefabName>` — loads a prefab from `Resources` and instantiates near the camera
- `setflag <key> <value>` — sets a choice flag (`bool`, `int`, `float`, or `string`)
- `teleport <x> <y> <z>` — moves the `Player` tagged object or `Camera.main`
- `shownav` — toggle NavMesh visualization (Editor only)

## Adding commands
Annotate a method with `[DevCommand("name", "description")]` on any `MonoBehaviour` (instance) or static method.

```csharp
public class ExampleDevCommands : MonoBehaviour
{
    [DevCommand("hello", "Replies with a greeting")]
    public string Hello(string[] args) { return "Hello"; }
}
```

Supported signatures:
- `string Handler(string[] args)` — return value is printed
- `void Handler(string[] args)`

## Auto-bootstrap
`Assets/Boot/DevBootstrap.cs` auto-instantiates the overlay in Editor/Development builds.
- Prefab: place at `Resources/Dev/DevOverlay.prefab`
- If missing, a minimal `DevConsole` is created so tests still run.

## Tests
See `Assets/Tests/DevConsoleTests.cs` for examples validating flag persistence and command discovery. Run via Unity Test Runner or CLI with `-runTests`.
