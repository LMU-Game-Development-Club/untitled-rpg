using UnityEngine;

// Name: Jam
// Date of Creation: 2/23/25
// Description: Sets up attributes for limbs 

public class LimbController : MonoBehaviour
{
    public float limbCurrentHealth;
    public float limbMaxHealth;
    public bool isDestroyed;
    public int turnsSinceDestruction;
    public int cooldownTime;
    public string uiInfo;
    public CS_LimbAttacks[] attacksArray;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (limbCurrentHealth <= 0)
        {
            isDestroyed = true;
        }

        if (isDestroyed & turnsSinceDestruction == cooldownTime)
        {
            isDestroyed = false;
            limbCurrentHealth = limbMaxHealth;
        }
    }

    void Attack()
    {
        // Function that handles each attack going out 
        // Player clicks button 1 --> Get info attacksArray[0] and apply
        // Player clicks button 2 --> Get info attacksArray[1] and apply
        // Player clicks button 3 --> Get info attacksArray[2] and apply
        // Subtract turn decay percentage accordingly 
     }
}
