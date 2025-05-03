using UnityEngine;

public class MI_DialogueTrigger : MonoBehaviour, IInteractable {
    [Header("Dialogue Reference")]
    public DialogueController dialogueController;
    public string startLineID = "0";

    private void Start() {
        if (dialogueController == null) {
            dialogueController = FindObjectOfType<DialogueController>();
            if (dialogueController == null) {
                Debug.LogError("No DialogueController found in scene.");
            }
        }
    }

    public void Interact() {
        if (dialogueController != null) {
            dialogueController.Play(startLineID);
        } else {
            Debug.LogWarning("DialogueController not set.");
        }
    }
}
