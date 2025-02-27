using UnityEngine;

public class EnemyInteractionZone : MonoBehaviour
{
    //enemy info is placed up here, which will be passed through for the fight.
     public GameObject[,] EnemiesEncountered;
     // this could be accessed in the inspector, and have each enemy type added as a game object

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("START ENEMY FIGHT HERE");
            Destroy(this.gameObject);
        }
    }
}
