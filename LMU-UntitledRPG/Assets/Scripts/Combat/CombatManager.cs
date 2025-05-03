using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    public GameObject enemy;
    public List<GameObject> inventoryItems;

    public GameObject head;
    public GameObject arm;

    public TextMeshProUGUI attack1;
    public TextMeshProUGUI attack2;

    public Button headSprite;
    public Button armSprite;
    public Button lightAttackButton;
    public Button heavyAttackButton;

    public Image TurnBarImage;
    public Image PlayerHealthBarImage;
    public Image EnemyHealthBarImage;

    public int maxPlayerHealth = 100;
    private int playerHealth;

    private float playerNextTurnPercent;

    private GameObject selectedLimb;
    private GameObject selectedAttack;

    void Start()
    {
        playerHealth = maxPlayerHealth;
        UpdatePlayerHealth();

        playerNextTurnPercent = 100f;

        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();

        lightAttackButton.onClick.AddListener(() => OnAttackSelected(0));
        heavyAttackButton.onClick.AddListener(() => OnAttackSelected(1));

        ArmSelected(armSprite);
    }

    void ArmSelected(Button limbButton)
    {
        string limbType = limbButton == headSprite ? "head" : "arm";
        selectedLimb = limbType == "head" ? head : arm;
        var limbScript = selectedLimb.GetComponent<MiloLimbScriptTemplate>();


        if (limbScript.attack1 == null || limbScript.attack2 == null)
        {
            Debug.LogError("One or both attacks are null on limb: " + selectedLimb.name);
        }
        attack1.text = limbScript.attack1.name;
        attack2.text = limbScript.attack2.name;
    }

    void OnAttackSelected(int attackIndex)
    {
        if (selectedAttack == null)
        {
            Debug.LogError("Selected attack is null");
        }

        if (selectedLimb == null)
            return;

        var limbScript = selectedLimb.GetComponent<MiloLimbScriptTemplate>();
        selectedAttack = attackIndex == 0 ? limbScript.attack1.gameObject : limbScript.attack2.gameObject;

        ExecuteAttack(selectedAttack.GetComponent<MiloAttackScriptTemplate>());
    }

    void ExecuteAttack(MiloAttackScriptTemplate attack)
    {
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        MiloHatScript hatScript = head.GetComponent<MiloHatScript>();

        float bonusDamage = hatScript != null ? hatScript.BonusDamage : 0f;

        enemyScript.health -= attack.damage + attack.elementDamage + bonusDamage;
        UpdateEnemyHealth();

        // Apply optional healing from hat
        if (hatScript != null && hatScript.HealingPerAttack > 0)
        {
            playerHealth = Mathf.Min(playerHealth + (int)hatScript.HealingPerAttack, maxPlayerHealth);
            Debug.Log("Player healed for " + hatScript.HealingPerAttack);
            UpdatePlayerHealth();
        }

        float turnDecayReduction = hatScript != null ? hatScript.TurnModifier : 0f;
        playerNextTurnPercent -= Mathf.Max(0, attack.turnDecay - turnDecayReduction);
        UpdateTurnBar();

        CheckEnemyDefeat();
        DetermineNextTurn();
    }

    void EnemyTurn()
    {
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        MiloAttackScriptTemplate chosenAttack = Random.Range(0, 2) == 0
            ? enemyScript.attack1.GetComponent<MiloAttackScriptTemplate>()
            : enemyScript.attack2.GetComponent<MiloAttackScriptTemplate>();

        MiloHatScript hatScript = head.GetComponent<MiloHatScript>();
        float defenseBonus = hatScript != null ? hatScript.BonusDefense : 0f;

        float totalDamage = Mathf.Max(0, (chosenAttack.damage + chosenAttack.elementDamage) - defenseBonus);

        playerHealth -= (int)totalDamage;
        Debug.Log("Enemy used " + chosenAttack.attackName + " for " + totalDamage + " damage." +
                  (defenseBonus > 0 ? " (Reduced by " + defenseBonus + " from hat)" : ""));

        UpdatePlayerHealth();
        CheckPlayerDefeat();
    }

    void UpdatePlayerHealth()
    {
        float fill = Mathf.Clamp01((float)playerHealth / maxPlayerHealth);
        PlayerHealthBarImage.fillAmount = fill;
    }

    void UpdateTurnBar()
    {
        float fill = Mathf.Clamp01(playerNextTurnPercent / 100f);
        TurnBarImage.fillAmount = fill;
    }

    void UpdateEnemyHealth()
    {
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        float fill = Mathf.Clamp01(enemyScript.health / 100f);
        EnemyHealthBarImage.fillAmount = fill;
    }

    void CheckEnemyDefeat()
    {
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        if (enemyScript.health <= 0)
        {
            Debug.Log("Enemy defeated!");

            GameManager.Instance.LoadEx();
        }
    }

    void CheckPlayerDefeat()
    {
        if (playerHealth <= 0)
        {
            Debug.Log("Player defeated!");
        }
    }

    void DetermineNextTurn()
    {
        if (playerNextTurnPercent <= UnityEngine.Random.Range(1, 101))
        {
            EnemyTurn();
            playerNextTurnPercent = 100f;
            UpdateTurnBar();
        }
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
    }
}