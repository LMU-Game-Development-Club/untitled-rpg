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
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();
        enemyScript.health -= attack.damage + attack.elementDamage;
        enemyHealth.text = "Enemy HP: " + enemyScript.health;

        playerNextTurnPercent -= attack.turnDecay;
        turnMeter.text = "Player turn chance: " + playerNextTurnPercent + "%";

        DetermineNextTurn();
    }

    void DetermineNextTurn()
    {
        if (playerNextTurnPercent <= UnityEngine.Random.Range(1, 101))
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
