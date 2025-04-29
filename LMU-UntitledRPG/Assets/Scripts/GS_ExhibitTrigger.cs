using UnityEngine;
using UnityEngine.SceneManagement;

public class GS_ExhibitTrigger : MonoBehaviour
{   
    [SerializeField] private int exhibitNumber;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player")) {
            Debug.Log($"Entering exhibit {exhibitNumber}");
            // SceneManager.LoadScene($"Exhibit{exhibitNumber}");
        }
    }
}
