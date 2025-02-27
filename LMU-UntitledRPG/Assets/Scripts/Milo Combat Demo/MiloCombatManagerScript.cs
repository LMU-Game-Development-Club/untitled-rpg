using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

public class MiloCombatManagerScript : MonoBehaviour
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
    private bool isPlayerTurn;
    private float playerNextTurnPercent;

    private GameObject selectedLimb;
    private GameObject selectedAttack;

    void Start()
    {
        isPlayerTurn = true;
        playerNextTurnPercent = 100f;

        headSprite.onClick.AddListener(() => OnLimbSelected(headSprite));
        armSprite.onClick.AddListener(() => OnLimbSelected(armSprite));
        lightAttackButton.onClick.AddListener(() => OnAttackSelected(0));
        heavyAttackButton.onClick.AddListener(() => OnAttackSelected(1));
    }

    void OnLimbSelected(Button limbButton)
    {
        string limbType = limbButton == headSprite ? "head" : "arm";
        var selectedLimb = limbType == "head" ? head : arm;
        var limbScript = selectedLimb.GetComponent<MiloLimbScriptTemplate>();

        limbDescription.text = limbScript.limbDescription;
        attack1.text = limbScript.attacks[0].name;
        attack2.text = limbScript.attacks[1].name;
    }

    void OnAttackSelected(int attackIndex)
    {
        if (selectedLimb == null)
        {
            return;
        }

        var limbScript = selectedLimb.GetComponent<MiloLimbScriptTemplate>();
        MiloAttackScriptTemplate attack = limbScript.attacks[attackIndex];
        selectedAttack = attack.gameObject;

        ExecuteAttack(selectedAttack.GetComponent<MiloAttackScriptTemplate>());
    }

    void ExecuteAttack(MiloAttackScriptTemplate attack)
    {
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();

        enemyScript.health -= attack.damage + attack.elementDamage;
        enemyHealth.text = "Enemy HP: " + enemyScript.health;

        playerNextTurnPercent -= attack.turnDecay;
        turnMeter.text = "Player turn chance: " + playerNextTurnPercent + "%";

        DetermineNextTurn();
    }

    void DetermineNextTurn()
    {
        // random turn logic here
        if (playerNextTurnPercent <= 0)
        {
            isPlayerTurn = false;
            playerNextTurnPercent = 100f;
            // Enemy turn logic here
        }
        else
        {
            isPlayerTurn = true;
        }
    }
}
