using UnityEngine;

public class MI_InteractableButton : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Button Pressed!");
    }
}
