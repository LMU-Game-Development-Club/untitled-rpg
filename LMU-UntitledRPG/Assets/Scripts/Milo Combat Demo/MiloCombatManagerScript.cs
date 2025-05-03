using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEditor.SearchService;


public class MiloCombatManagerScript : MonoBehaviour
{
    public GameObject enemy;
    public List<GameObject> inventoryItems;
    public GameObject head;
    public GameObject arm;
    // public TextMeshProUGUI limbDescription;
    public TextMeshProUGUI attack1;
    public TextMeshProUGUI attack2;
    public TextMeshProUGUI enemyHealth;
    public TextMeshProUGUI turnMeter;
    public Button headSprite;
    public Button armSprite;
    public Button lightAttackButton;
    public Button heavyAttackButton;
    public int lives;
    private float playerNextTurnPercent;
    // public GameObject equipmentCanvas;

    private GameObject selectedLimb;
    private GameObject selectedAttack;


    public Image TurnBarImage;
    
    // TODO: Add limb cooldown logic
    void Start()
    {
        playerNextTurnPercent = 100f;

        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        enemyHealth.text = "Enemy HP: " + enemyScript.health;

        // headSprite.onClick.AddListener(OnLimbClicked);
        // armSprite.onClick.AddListener(OnLimbClicked);

        lightAttackButton.onClick.AddListener(() => OnAttackSelected(0));
        heavyAttackButton.onClick.AddListener(() => OnAttackSelected(1));

        // only arms have attacks now
        ArmSelected(armSprite);
    }

    // void OnLimbClicked()
    // {
    //     equipmentCanvas.SetActive(true);
    // }

    void ArmSelected(Button limbButton)
    {
        string limbType = limbButton == headSprite ? "head" : "arm";
        selectedLimb = limbType == "head" ? head : arm;
        var limbScript = selectedLimb.GetComponent<MiloLimbScriptTemplate>();

        // limbDescription.text = limbScript.limbDescription;
        attack1.text = limbScript.attack1.name;
        attack2.text = limbScript.attack2.name;
    }

    void OnAttackSelected(int attackIndex)
    {
        if (selectedLimb == null)
        {
            return;
        }
        
        var limbScript = selectedLimb.GetComponent<MiloLimbScriptTemplate>();
        if (attackIndex == 0) {
            selectedAttack = limbScript.attack1.gameObject;
        }
        else {
            selectedAttack = limbScript.attack2.gameObject;
        }

        ExecuteAttack(selectedAttack.GetComponent<MiloAttackScriptTemplate>());
    }

    void ExecuteAttack(MiloAttackScriptTemplate attack)
    {
        // Get enemy script reference
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        
        // Get hat component if available (assuming it's attached to the player or a specific limb)
        MiloHatScript hatScript = head.GetComponent<MiloHatScript>();
        float bonusDamage = hatScript != null ? hatScript.BonusDamage : 0f;
        
        // Apply damage with hat bonus
        enemyScript.health -= attack.damage + attack.elementDamage + bonusDamage;
        enemyHealth.text = "Enemy HP: " + enemyScript.health;
        
        // Apply hat healing effect to arm if hat exists
        if (hatScript != null && hatScript.HealingPerAttack > 0)
        {
            var armScript = arm.GetComponent<MiloLimbScriptTemplate>();
            armScript.limbHealth = Mathf.Min(armScript.limbHealth + hatScript.HealingPerAttack, armScript.limbMaxHealth);
            Debug.Log(armScript.limbName + " healed for " + hatScript.HealingPerAttack + " health.");
            
            // If arm was broken and nowhas health, restore it
            if (armScript.limbHealth > 0 && armScript.isBroken)
            {
                armScript.isBroken = false;
                Debug.Log(armScript.limbName + " is no longer broken.");
            }
        }
        
        // Reduce turn decay by hat turn modifier if hat exists
        float turnDecayReduction = hatScript != null ? hatScript.TurnModifier : 0f;
        playerNextTurnPercent -= Mathf.Max(0, attack.turnDecay - turnDecayReduction);
        turnMeter.text = "Player turn chance: " + playerNextTurnPercent + "%";
        UpdateTurnBar();

        CheckEnemyDefeat();
        DetermineNextTurn();
    }

    void CheckEnemyDefeat()
    {
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        if (enemyScript.health <= 0)
        {
            // Load a temporary scene or victory screen
            GameManager.Instance.LoadExhibit1();
        }
    }

    void UpdateTurnBar(){
        float fill = Mathf.Clamp01(playerNextTurnPercent / 100f);
        TurnBarImage.fillAmount = fill;
    }

    void UpdateHealthBar(int health){
        float fill = Mathf.Clamp01(health / 100f);
        TurnBarImage.fillAmount = fill;
    }

    void CheckPlayerDefeat()
    {
        if (lives <= 0)
        {
            // if player is defeated, end the combat or show game over message
            Debug.Log("Player defeated!");
        }
    }

    void DetermineNextTurn()
    {
        if (playerNextTurnPercent <= UnityEngine.Random.Range(1, 101))
        {
            EnemyTurn();

            playerNextTurnPercent = 100f;
            turnMeter.text = "Player turn chance: " + playerNextTurnPercent + "%";
            UpdateTurnBar();
        }
    }

    void EnemyTurn()
    {
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        MiloAttackScriptTemplate chosenAttack = UnityEngine.Random.Range(0, 2) == 0
            ? enemyScript.attack1.GetComponent<MiloAttackScriptTemplate>()
            : enemyScript.attack2.GetComponent<MiloAttackScriptTemplate>();
        var chosenLimbScript = arm.GetComponent<MiloLimbScriptTemplate>();
        
        // Get hat defense bonus if available
        MiloHatScript hatScript = head.GetComponent<MiloHatScript>();
        float defenseBonus = hatScript != null ? hatScript.BonusDefense : 0f;
        
        // Calculate damage after defense reduction
        float totalDamage = Mathf.Max(0, (chosenAttack.damage + chosenAttack.elementDamage) - defenseBonus);
        
        // Deal damage to the chosen limb
        chosenLimbScript.limbHealth -= totalDamage;
        Debug.Log("Enemy used " + chosenAttack.attackName + " for " + totalDamage +
                  " damage to the player's " + chosenLimbScript.limbName + 
                  (defenseBonus > 0 ? " (Reduced by " + defenseBonus + " from hat)" : "") + ".");

        // If limb is reduced to zero health, start its cooldown
        if (chosenLimbScript.limbHealth <= 0)
        {
            chosenLimbScript.limbHealth = 0;
            chosenLimbScript.isBroken = true;
            chosenLimbScript.limbCooldown = chosenLimbScript.limbMaxCooldown;
            Debug.Log(chosenLimbScript.limbName + " is broken for " + chosenLimbScript.limbCooldown + " turns.");
        }

        CheckPlayerDefeat();
    }

    public void limbChanged(string limbType, string limbName)
    {
        GameObject limb = inventoryItems.Find(item => item.name == limbName);
        if (limb == null)
        {
            Debug.LogWarning("Limb with name " + limbName + " not found in inventory.");
            return;
        }
        if (limbType == "head")
        {
            head = limb;
        }
        else if (limbType == "arm")
        {
            arm = limb;
            ArmSelected(armSprite);
        }
        else
        {
            Debug.LogWarning("Invalid limb type: " + limbType);
        }
        // equipmentCanvas.SetActive(false);
    }
}
