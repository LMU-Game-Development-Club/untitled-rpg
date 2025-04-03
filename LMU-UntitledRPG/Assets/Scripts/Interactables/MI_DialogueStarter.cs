using UnityEngine;

public class MI_DialogueStarter : MonoBehaviour, IInteractable
{
    private UI_DialogueController dialogueController;
    public string dialogueName;
    private void Start() {
        UI_DialogueManager d = UI_DialogueManager.Instance;
        dialogueController = d.transform.Find("DialogueController").GetComponent<UI_DialogueController>();
    }
    public void Interact() {
        dialogueController.StartDialogue(dialogueName);
    }

}
