using UnityEngine;

// Name: Jacob Mendoza
// Date of Creation: 4/8/25
// Description: This file was written in order to test the health bar UI Element's functionality
// This can be deleted later, I just needed a way to test to make sure the code I wrote worked.
public class TestingButton : MonoBehaviour
{
    public float damageTotal = 50.0f;
    public UI_TurnBar turnBar;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DamagePlayer()
    {
        turnBar.currentPlayerChance = turnBar.currentPlayerChance - damageTotal;
    }
}
