using UnityEngine;
using UnityEngine.UI;

// Name: Jacob Mendoza
// Date of Creation: 4/8/25
// Description: This file handles how the Turnbar UI works. This code for this is relatively
// simple so it can be modified as needed. What's currently set up in the scene this script is attached to is that the 
// health bar shrinks according to how much health the player currently has (if the player has 85 out of 100 health left,
// the bar itself multiplies its width by 0.85 in order to shrink down).

public class UI_TurnBar : MonoBehaviour
{
    public Image backgroundImage;
    public Image foregroundImage;
    public float maxPlayerChance = 100.0f;
    public float currentPlayerChance = 100.0f;
    public float enemyChance = 0.0f;
    public float testingDamage = 50.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTurnChances();
    }

    private void UpdateTurnChances()
    {
        float turnPercentage = currentPlayerChance / maxPlayerChance;
        RectTransform foregroundRect = foregroundImage.GetComponent<RectTransform>();
        foregroundRect.sizeDelta = new Vector2(backgroundImage.rectTransform.sizeDelta.y * turnPercentage, foregroundRect.sizeDelta.y);
    }

    public void TakeDamage()
    {
        currentPlayerChance -= testingDamage;
    }
}
