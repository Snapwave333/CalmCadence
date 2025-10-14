# Interactions & Choice Manager

## IInteractable
- OnLook(): called when the player looks at the object
- OnInteract(): called when the player interacts
- GetHint(): returns a short hint string

## InteractableBase
Base class implementing IInteractable with a configurable hint.

## ChoiceManager
- SetFlag(key, value)
- GetFlag<T>(key)
- SubscribeToFlagChanges(key, callback)

Example:
```csharp
public class Door : InteractableBase
{
    public override void OnInteract()
    {
        ChoiceManager.Instance.SetFlag("door_open", true);
    }
}
```
