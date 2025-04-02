using UnityEngine;

public class InteractableObjectZone : MonoBehaviour
{
    public GameObject TextObject;
    private bool playerInTrigger = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TextObject.SetActive(true);
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TextObject.SetActive(false);
            playerInTrigger = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E in interaction zone.");
        }
    }
}
