using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

public class JamCombatManagerScript : MonoBehaviour
{
    public GameObject enemy;
    public GameObject head;
    public GameObject arm;
    public TextMeshProUGUI limbDescription;
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

    public Image backgroundImage;
    public Image foregroundImage;

    private GameObject selectedLimb;
    private GameObject selectedAttack;

    // TODO: Add limb cooldown logic
    void Start()
    {
        playerNextTurnPercent = 100f;

        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        enemyHealth.text = "Enemy HP: " + enemyScript.health;

        headSprite.onClick.AddListener(() => OnLimbSelected(headSprite));
        armSprite.onClick.AddListener(() => OnLimbSelected(armSprite));
        lightAttackButton.onClick.AddListener(() => OnAttackSelected(0));
        heavyAttackButton.onClick.AddListener(() => OnAttackSelected(1));
    }

    void OnLimbSelected(Button limbButton)
    {
        string limbType = limbButton == headSprite ? "head" : "arm";
        selectedLimb = limbType == "head" ? head : arm;
        var limbScript = selectedLimb.GetComponent<MiloLimbScriptTemplate>();

        limbDescription.text = limbScript.limbDescription;
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
        if (attackIndex == 0)
        {
            selectedAttack = limbScript.attack1.gameObject;
        }
        else
        {
            selectedAttack = limbScript.attack2.gameObject;
        }

        ExecuteAttack(selectedAttack.GetComponent<MiloAttackScriptTemplate>());
    }

    void ExecuteAttack(MiloAttackScriptTemplate attack)
    {
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        enemyScript.health -= attack.damage + attack.elementDamage;
        enemyHealth.text = "Enemy HP: " + enemyScript.health;

        playerNextTurnPercent -= attack.turnDecay;
        turnMeter.text = "Player turn chance: " + playerNextTurnPercent + "%";

        CheckEnemyDefeat();
        DetermineNextTurn();
    }

    void CheckEnemyDefeat()
    {
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        if (enemyScript.health <= 0)
        {
            // if enemy is defeated, end the combat or show victory message
            Debug.Log("Enemy defeated!");
        }
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
        }
    }

    void EnemyTurn()
    {
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        MiloAttackScriptTemplate chosenAttack = UnityEngine.Random.Range(0, 2) == 0
            ? enemyScript.attack1.GetComponent<MiloAttackScriptTemplate>()
            : enemyScript.attack2.GetComponent<MiloAttackScriptTemplate>();

        // Randomly choose a limb (head or arm) to apply damage to
        var chosenPlayerLimb = UnityEngine.Random.Range(0, 2) == 0 ? head : arm;
        var chosenLimbScript = chosenPlayerLimb.GetComponent<MiloLimbScriptTemplate>();

        // Deal damage to the chosen limb
        chosenLimbScript.limbHealth -= chosenAttack.damage + chosenAttack.elementDamage;
        Debug.Log("Enemy used " + chosenAttack.attackName + " for " + (chosenAttack.damage + chosenAttack.elementDamage) +
                  " damage to the player's " + chosenLimbScript.limbName + ".");

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

    void Update()
    {
        RectTransform foregroundRect = foregroundImage.GetComponent<RectTransform>();
        foregroundRect.sizeDelta = new Vector2(backgroundImage.rectTransform.sizeDelta.y * (playerNextTurnPercent/100.0f), foregroundRect.sizeDelta.y);
    }
}

