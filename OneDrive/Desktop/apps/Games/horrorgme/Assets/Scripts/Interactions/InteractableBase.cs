using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
	[SerializeField] private string hint = "Interact";

	public virtual void OnLook() { }
	public abstract void OnInteract();
	public virtual string GetHint() { return hint; }
}
