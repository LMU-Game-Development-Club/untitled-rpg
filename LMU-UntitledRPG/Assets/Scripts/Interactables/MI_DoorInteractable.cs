using UnityEngine;

public class MI_DoorInteractable : MonoBehaviour, IInteractable
{
    public Transform targetLocation;
    public GameObject player;
    public bool canEnter = false;

    [Header("Require Dialogue")]
    public DialogueController dialogueController;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No GameObject with tag 'Player' found!");
        }

        if (dialogueController != null)
        {
            dialogueController.OnDialogueEnd.AddListener(EnableDoor);
        }
        else
        {
            Debug.LogWarning("DialogueController not assigned to door!");
        }
    }

    public void Interact()
    {
        if (player != null && targetLocation != null && canEnter)
        {
            player.transform.position = targetLocation.position;
        }
        else if (!canEnter)
        {
            Debug.Log("Player can't enter — dialogue must be finished.");
        }
        else
        {
            Debug.LogWarning("Player or Target Location not set!");
        }
    }

    private void EnableDoor()
    {
        canEnter = true;
        Debug.Log("Door unlocked after dialogue.");
    }
}
