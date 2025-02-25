using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

public class MiloCombatManagerScript : MonoBehaviour
{
    public GameObject[] enemies;
    public GameObject[] limbs;
    public TextMeshProUGUI limbDescription;
    public TextMeshProUGUI attack1;
    public TextMeshProUGUI attack2;
    public TextMeshProUGUI enemyHealth;
    public TextMeshProUGUI turnMeter;
    public Button headSprite;
    public Button armSprite;
    public Button[] attackButtons;
    public int lives;
    private bool isPlayerTurn;
    private float playerNextTurnPercent;

    private GameObject selectedLimb;
    private GameObject selectedAttack;

    void Start()
    {
        Debug.Log("Start method called");
        isPlayerTurn = true;
        playerNextTurnPercent = 100f;
        Debug.Log("Player turn initialized: " + isPlayerTurn);
        Debug.Log("Player next turn percent initialized: " + playerNextTurnPercent);

        headSprite.onClick.AddListener(() => OnLimbSelected(headSprite));
        armSprite.onClick.AddListener(() => OnLimbSelected(armSprite));
        Debug.Log("Head and arm sprite buttons initialized");
        attackButtons[0].onClick.AddListener(() => OnAttackSelected(0));
        attackButtons[1].onClick.AddListener(() => OnAttackSelected(1));
        Debug.Log("Attack buttons initialized");
    }

    void OnLimbSelected(Button limbButton)
    {
        Debug.Log("OnLimbSelected method called with limbButton: " + limbButton);
        int limbIndex = limbButton == headSprite ? 0 : 1;
        Debug.Log("Limb index determined: " + limbIndex);

        var limbScript = limbs[limbIndex].GetComponent<MiloLimbScriptTemplate>();
        Debug.Log("Limb script component retrieved");

        selectedLimb = limbs[limbIndex];
        Debug.Log("Selected limb: " + selectedLimb.name);

        Debug.Log("Changing: " + limbDescription.text);
        Debug.Log("To: " + limbScript.limbDescription);
        limbDescription.text = limbScript.limbDescription;
        attack1.text = limbScript.attacks[0].name;
        attack2.text = limbScript.attacks[1].name;
        Debug.Log("Text boxes updated with limb description and attacks");
    }

    void OnAttackSelected(int attackIndex)
    {
        Debug.Log("OnAttackSelected method called with attackIndex: " + attackIndex);
        if (selectedLimb == null)
        {
            Debug.LogWarning("No limb selected, returning from OnAttackSelected");
            return;
        }

        var limbScript = selectedLimb.GetComponent<MiloLimbScriptTemplate>();
        MiloAttackScriptTemplate attack = limbScript.attacks[attackIndex];
        selectedAttack = attack.gameObject;
        Debug.Log("Selected attack: " + selectedAttack.name);

        ExecuteAttack(selectedAttack.GetComponent<MiloAttackScriptTemplate>());
    }

    void ExecuteAttack(MiloAttackScriptTemplate attack)
    {
        Debug.Log("ExecuteAttack method called with attack: " + attack.name);
        var enemy = enemies[0]; // Assuming single enemy for simplicity
        var enemyScript = enemy.GetComponent<MiloEnemyScriptTemplate>();

        enemyScript.health -= attack.damage + attack.elementDamage;
        Debug.Log("Enemy health after attack: " + enemyScript.health);

        enemyHealth.text = "Enemy HP: " + enemyScript.health;
        Debug.Log("Enemy HP text box updated");

        playerNextTurnPercent -= attack.turnDecay;
        turnMeter.text = "Player turn chance: " + playerNextTurnPercent + "%";
        Debug.Log("Player next turn percent after attack: " + playerNextTurnPercent);

        DetermineNextTurn();
    }

    void DetermineNextTurn()
    {
        // need to implement random turn + enemy attack selection
        if (playerNextTurnPercent <= 0)
        {
            isPlayerTurn = false;
            playerNextTurnPercent = 100f;
            Debug.Log("Player turn ended, enemy turn starts");
            // Enemy turn logic here
        }
        else
        {
            isPlayerTurn = true;
            Debug.Log("Player turn continues");
        }
    }
}
