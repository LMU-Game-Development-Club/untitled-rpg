using UnityEngine;

public class MI_DoorInteractable : MonoBehaviour, IInteractable
{
    //Position to move the character to
    public Transform targetLocation;
    public GameObject player;
    public bool canEnter;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No GameObject with tag 'Player' found!");
        }
        canEnter = true;
    }

    public void Interact()
    {
        if (player != null && targetLocation != null && canEnter)
        {
            player.transform.position = targetLocation.position;
            
        }
        else if (!canEnter)
        {
            Debug.Log("Player Cant enter");
        }
        else
        {
            Debug.LogWarning("Player or Target Location not set!");
        }
    }
}
