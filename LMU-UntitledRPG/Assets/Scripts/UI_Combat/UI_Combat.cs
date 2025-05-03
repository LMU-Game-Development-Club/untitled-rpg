using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Combat : MonoBehaviour {
    public enum WeaponType {
        Sword,
        Crossbow,
        Staff
    }

    // Names must be unique
    [Serializable]
    public struct Enemy {
        public Enemy(string a, List<Sprite> b, List<Sprite> c, List<Sprite> d, List<string> e, WeaponType f, float g) {
            name = a;
            idleSprites = b;
            hitSprites = c;
            deathSprites = d;
            actionSet = e;
            weapon = f;
            health = g;
        }
        public string name;
        public List<Sprite> idleSprites;
        public List<Sprite> hitSprites;
        public List<Sprite> deathSprites;
        public List<string> actionSet;
        public WeaponType weapon;
        public float health;
    }

    private static Enemy emptyEnemy = new Enemy("Empty", null, null, null, null, WeaponType.Sword, 0f); 

    public enum ActionType {
        Attack,
        Other
    }

    public enum PassiveEffect {
        Burn,
        Heal,
        Shield,
        None
    }

    // Name must be unique
    [Serializable]
    public struct Action {
        public Action(string a, string b, ActionType c, float d, PassiveEffect e, float f, float g) {
            name = a;
            description = b;
            actionType = c;
            actionIntensity = d;
            passive = e;
            passiveIntensity = f;
            turnBarDecay = g;
        }
        public string name;
        public string description;
        public ActionType actionType;
        public float actionIntensity;
        public PassiveEffect passive;
        public float passiveIntensity;
        public float turnBarDecay;
    }
    private static Action emptyAction = new Action("Empty", null, ActionType.Other, 0f, PassiveEffect.None, 0f, 0f); 

    [Header("PlayerInfo")]
    public string equippedLimb;
    public string equippedHat;
    [Header("Refs")]
    public UI_EquipmentController EquipmentController;
    [Space]
    [Header("Set Avaliable Enemies")]
    [Space]
    public List<Enemy> Enemies;
    [Header("Set Avaliable Actions")]
    [Space]
    public List<Action> Actions;
    [Space]
    [Header("Effects")]
    public GameObject HitEffectUIPrefab;
    [Space]
    [Header("Weapon Prefabs")]
    public GameObject SwordPrefab;
    public GameObject CrossbowPrefab;
    public GameObject StaffPrefab;
    [Space]
    [Header("Weapon Animations")]
    public Vector3 SwordStartPosition;
    public Vector3 SwordStartRotation;
    public Vector3 SwordEndPosition;
    public Vector3 SwordEndRotation;
    public Vector3 CrossbowStartPosition;
    public Vector3 CrossbowStartRotation;
    public Vector3 CrossbowEndPosition;
    public Vector3 CrossbowEndRotation;
    public Vector3 StaffStartPosition;
    public Vector3 StaffStartRotation;
    public Vector3 StaffEndPosition;
    public Vector3 StaffEndRotation;

    public UnityEvent PlayerLoseCombat;
    public UnityEvent PlayerWinCombat;

    private Vector3 swordPosition;
    private Vector3 swordRotation;
    private Vector3 crossbowPosition;
    private Vector3 crossbowRotation;
    private Vector3 staffPosition;
    private Vector3 staffRotation;

    [Space]
    [Header("Debug")]
    public bool DebugMode;

    enum CombatState {
        PlayerMove,
        PlayerActionSequence,
        EnemyMove,
        EnemyActionSequence
    }

    enum PlayerGUI {
        MainView,
        ActionView,
        EquipmentView,
        NotPlayerMoveView
    }

    enum EnemyAnimation {
        Idle,
        Hurt,
        Death,
        Attack,
    }

    private System.Random prng;

    private EnemyAnimation enemyAnimation;
    private int enemyAnimationFrame;
    private float enemyAnimationInterval = 0.166f; //6 fps
    private float enemyAttackAnimationInterval = 0.0416f; //60 fps
    private float enemyAnimationStep;

    private float playerActionSequenceLength = 1f;
    private float playerActionSequenceEnd;
    private float enemyActionSequenceLength = 1f;
    private float enemyActionSequenceEnd;
    private float lerpAmount;

    private CombatState combatState;
    private PlayerGUI playerGUI;

    private UI_CombatBar enemyHealthBar;
    private UI_CombatBar healthBar;
    private UI_CombatBar actionBar;
    private Image enemyRenderer;

    private Canvas combatCanvas;
    private GameObject attackButton;
    private GameObject equipmentButton;
    private GameObject backButton;
    private GameObject actionMoveHat;
    private GameObject actionMoveLimb;
    private GameObject crossbow;
    private GameObject staff;
    private GameObject sword;
    private GameObject equipItemsWarning;

    private float localTime;

    private List<GameObject> objectsToDumpAfterTransition;
    
    private Enemy enemy;
    private bool enemySet;

    private Action[] playerActions;
    private Action playerSelectedAction;
    private Action enemySelectedAction;

    private float playerHealth;
    private float playerHealthMax = 100;
    private float playerTurnChance;
    private float playerTurnDefault = 1;
    private float enemyHealth;

    public void StartCombat(string enemyName) {
        enemy = FindEnemy(enemyName);
        if (enemy.name != "Empty") {
            enemyRenderer.sprite = enemy.idleSprites[0];
            enemySet = true;
            switch (enemy.weapon) {
                case WeaponType.Sword:
                    SetObjsActive(true, sword);
                    SetObjsActive(false, staff, crossbow);
                    break;
                case WeaponType.Staff:
                    SetObjsActive(true, staff);
                    SetObjsActive(false, sword, crossbow);
                    break;
                case WeaponType.Crossbow:
                    SetObjsActive(true, crossbow);
                    SetObjsActive(false, staff, sword);
                    break;
            
            }
            playerHealth = playerHealthMax;
            playerTurnChance = playerTurnDefault;
            enemyHealth = enemy.health;

            TransitionCombatState(CombatState.PlayerMove);
            TransitionPlayerGUI(PlayerGUI.MainView);
            TransitionEnemyAnimation(EnemyAnimation.Idle);
            HandleEquipmentChange();

            combatCanvas.enabled = true;
        } else {
            Log($"{enemyName} not found so combat did not start");
        }
    }

    void Start() {
        prng = new System.Random(0);

        if (PlayerLoseCombat == null) {
            PlayerLoseCombat = new UnityEvent();
        }
        if (PlayerWinCombat == null) {
            PlayerWinCombat = new UnityEvent();
        }

        combatCanvas = GetComponent<Canvas>();

        enemyHealthBar = transform.Find("EnemyHealthBar").GetComponent<UI_CombatBar>();
        enemyHealthBar.SetBarFill(1);

        healthBar = transform.Find("HealthBar").GetComponent<UI_CombatBar>();
        healthBar.SetBarFill(1);

        actionBar = transform.Find("TurnBar").GetComponent<UI_CombatBar>();
        actionBar.SetBarFill(1);

        enemyRenderer = transform.Find("EnemyRenderer").GetComponent<Image>();

        attackButton = transform.Find("AttackButton").gameObject;
        attackButton.GetComponent<UI_CombatOnClick>().onClick.AddListener(() => TransitionPlayerGUI(PlayerGUI.ActionView));

        equipmentButton = transform.Find("EquipmentButton").gameObject;
        equipmentButton.GetComponent<UI_CombatOnClick>().onClick.AddListener(() => TransitionPlayerGUI(PlayerGUI.EquipmentView));

        backButton = transform.Find("BackButton").gameObject;
        backButton.GetComponent<UI_CombatOnClick>().onClick.AddListener(() => TransitionPlayerGUI(PlayerGUI.MainView));

        EquipmentController.EquipmentExit.AddListener(() => TransitionPlayerGUI(PlayerGUI.MainView));
        EquipmentController.EquipmentChanged.AddListener(() => HandleEquipmentChange());

        actionMoveHat = transform.Find("ActionMoveHat").gameObject;
        actionMoveHat.GetComponent<UI_CombatOnClick>().onClick.AddListener(() => PlayerPickedAction1());

        actionMoveLimb = transform.Find("ActionMoveLimb").gameObject;
        actionMoveLimb.GetComponent<UI_CombatOnClick>().onClick.AddListener(() => PlayerPickedAction2());

        equipItemsWarning = transform.Find("EquipItemsWarning").gameObject;

        crossbow = transform.Find("Crossbow").gameObject;
        staff = transform.Find("Staff").gameObject;
        sword = transform.Find("Sword").gameObject;

        swordPosition = sword.transform.localPosition; 
        swordRotation = sword.transform.localRotation.eulerAngles;
        crossbowPosition = crossbow.transform.localPosition; 
        crossbowRotation = crossbow.transform.localRotation.eulerAngles;
        staffPosition = staff.transform.localPosition; 
        staffRotation = staff.transform.localRotation.eulerAngles;

        // Temp
        enemyRenderer.sprite = Enemies[0].idleSprites[0];

        localTime = 0;

        objectsToDumpAfterTransition = new List<GameObject>();

        TransitionCombatState(CombatState.PlayerMove);
        TransitionPlayerGUI(PlayerGUI.MainView);
        TransitionEnemyAnimation(EnemyAnimation.Idle);
        HandleEquipmentChange();

        combatCanvas.enabled = false;
    }

    private void TransitionCombatState(CombatState newCombatState) {
        if (objectsToDumpAfterTransition.Count > 0) {
            foreach ( GameObject obj in objectsToDumpAfterTransition) {
                Destroy(obj);
            }
            objectsToDumpAfterTransition = new List<GameObject>();
        }

        combatState = newCombatState;

        switch (combatState) {
            case CombatState.PlayerMove:
                TransitionPlayerGUI(PlayerGUI.MainView);
                break;
            case CombatState.PlayerActionSequence:
                TransitionEnemyAnimation(EnemyAnimation.Hurt);
                playerActionSequenceEnd = localTime + playerActionSequenceLength;
                break;
            case CombatState.EnemyMove:
                int enemyMoveIndex = prng.Next(enemy.actionSet.Count);
                enemySelectedAction = FindAction(enemy.actionSet[enemyMoveIndex]);
                if (enemySelectedAction.name == "Empty") {
                    Log($"Enemy did not make a move cause {enemy.actionSet[enemyMoveIndex]} was not found in Actions");
                }
                TransitionCombatState(CombatState.EnemyActionSequence);
                break;
            case CombatState.EnemyActionSequence:
                TransitionEnemyAnimation(EnemyAnimation.Attack);
                enemyActionSequenceEnd = localTime + enemyActionSequenceLength;
                break;
        }
    }

    private void TransitionPlayerGUI(PlayerGUI newPlayerGUI) {
        if (combatState != CombatState.PlayerMove) {
            return;
        }

        switch (newPlayerGUI) {
            case PlayerGUI.MainView:
                SetObjsActive(true, attackButton, equipmentButton);
                SetObjsActive(false, backButton, actionMoveHat, actionMoveLimb, equipItemsWarning, EquipmentController.gameObject);
                break;
            case PlayerGUI.ActionView:
                SetObjsActive(false, attackButton, equipmentButton);
                SetObjsActive(true, backButton, actionMoveHat, actionMoveLimb);

                bool hatMovePresent = true;
                bool limbMovePresent = true;

                if (playerActions[0].name == "Empty") {
                    SetObjsActive(false, actionMoveHat);
                    hatMovePresent = false;
                }
                if (playerActions[1].name == "Empty") {
                    SetObjsActive(false, actionMoveLimb);
                    limbMovePresent = false;
                }

                if (!hatMovePresent && !limbMovePresent) {
                    SetObjsActive(true, equipItemsWarning);
                } else {
                    SetObjsActive(false, equipItemsWarning);
                }

                break;
            case PlayerGUI.EquipmentView:
                SetObjsActive(false, attackButton, equipmentButton, backButton);
                SetObjsActive(true, EquipmentController.gameObject);
                break;
            case PlayerGUI.NotPlayerMoveView:
                SetObjsActive(false, attackButton, equipmentButton, backButton, actionMoveHat, actionMoveLimb, EquipmentController.gameObject, equipItemsWarning);
                break;
        }

        playerGUI = newPlayerGUI;
    }

    private void TransitionEnemyAnimation(EnemyAnimation newEnemyAnimation) {
        enemyAnimation = newEnemyAnimation;
        enemyAnimationFrame = 0;
        enemyAnimationStep = localTime + enemyAnimationInterval;
        lerpAmount = 0;

        switch (enemy.weapon) {
            case WeaponType.Sword:
                sword.transform.localPosition = swordPosition; 
                sword.transform.localRotation = Quaternion.Euler(swordRotation);
                break;
            case WeaponType.Staff:
                staff.transform.localPosition = staffPosition; 
                staff.transform.localRotation = Quaternion.Euler(staffRotation);
                break;
            case WeaponType.Crossbow:
                crossbow.transform.localPosition = crossbowPosition; 
                crossbow.transform.localRotation = Quaternion.Euler(crossbowRotation);
                break;
        }
    }

    private void PlayerPickedAction1() {
        TransitionPlayerGUI(PlayerGUI.NotPlayerMoveView);
        playerSelectedAction = playerActions[0];
        TransitionCombatState(CombatState.PlayerActionSequence);
    }

    private void PlayerPickedAction2() {
        TransitionPlayerGUI(PlayerGUI.NotPlayerMoveView);
        playerSelectedAction = playerActions[1];
        TransitionCombatState(CombatState.PlayerActionSequence);
    }

    private void HandleCombatState(CombatState combatState) {
        switch (combatState) {
            case CombatState.PlayerMove:
                break;
            case CombatState.PlayerActionSequence:
                if (localTime > playerActionSequenceEnd) {
                    ProcessPlayerAction();
                    TransitionCombatState(CombatState.EnemyMove);
                }
                break;
            case CombatState.EnemyMove:
                break;
            case CombatState.EnemyActionSequence:
                if (localTime > enemyActionSequenceEnd) {
                    ProcessEnemyAction();
                    TransitionCombatState(CombatState.PlayerMove);
                    TransitionEnemyAnimation(EnemyAnimation.Idle);
                }
                break;
        }
    }

    private void HandleGUI(PlayerGUI playerGUI) {
        switch (playerGUI) {
            case PlayerGUI.MainView:
                break;
            case PlayerGUI.ActionView:
                break;
            case PlayerGUI.EquipmentView:
                break;
            case PlayerGUI.NotPlayerMoveView:
                break;
        }
    }

    private void HandleEquipmentChange() {
        Action hatAction = emptyAction;
        Action limbAction = emptyAction;

        if (EquipmentController.HeadSlot.name != "Empty") {
            hatAction = FindAction(EquipmentController.HeadSlot.action);
        }
        if (EquipmentController.HandSlot.name != "Empty") {
            limbAction = FindAction(EquipmentController.HandSlot.action);
        }

        actionMoveHat.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = hatAction.name;
        actionMoveHat.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = hatAction.description;

        actionMoveLimb.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = limbAction.name;
        actionMoveLimb.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = limbAction.description;

        playerActions = new Action[2]{hatAction, limbAction};
    }

    private void HandleEnemyAnimation() {
        if (!enemySet) {
            return;
        }
        if (localTime > enemyAnimationStep) {
            if (enemyAnimation == EnemyAnimation.Attack) {
                enemyAnimationStep = localTime + enemyAttackAnimationInterval;
            } else {
                enemyAnimationStep = localTime + enemyAnimationInterval;
            }
            switch (enemyAnimation) {
                case EnemyAnimation.Idle:
                    enemyRenderer.color = new Color(1, 1, 1, 1);
                    if (enemyAnimationFrame < enemy.idleSprites.Count) {
                        enemyRenderer.sprite = enemy.idleSprites[enemyAnimationFrame++];
                    } else {
                        enemyAnimationFrame = 0;
                        enemyRenderer.sprite = enemy.idleSprites[enemyAnimationFrame++];
                    }
                    break;
                case EnemyAnimation.Hurt:
                    enemyRenderer.color = new Color(0.5f, 0, 0, 1);
                    if (enemyAnimationFrame < enemy.hitSprites.Count) {
                        enemyRenderer.sprite = enemy.hitSprites[enemyAnimationFrame++];
                    } else {
                        TransitionEnemyAnimation(EnemyAnimation.Idle);
                    }
                    break;
                case EnemyAnimation.Death:
                    enemyRenderer.color = new Color(1, 1, 1, 1);
                    if (enemyAnimationFrame < enemy.deathSprites.Count) {
                        enemyRenderer.sprite = enemy.deathSprites[enemyAnimationFrame++];
                    }
                    break;
                case EnemyAnimation.Attack:
                    enemyRenderer.color = new Color(1, 1, 1, 1);
                    lerpAmount += enemyAnimationInterval/enemyActionSequenceLength;
                    switch (enemy.weapon) {
                        case WeaponType.Sword:
                            sword.transform.localPosition = Vector3.Lerp(SwordStartPosition, SwordEndPosition, lerpAmount);
                            sword.transform.localRotation = Quaternion.Euler(Vector3.Lerp(SwordStartRotation, SwordEndRotation, lerpAmount));
                            break;
                        case WeaponType.Staff:
                            staff.transform.localPosition = Vector3.Lerp(StaffStartPosition, StaffEndPosition, lerpAmount);
                            staff.transform.localRotation = Quaternion.Euler(Vector3.Lerp(StaffStartRotation, StaffEndRotation, lerpAmount));
                            break;
                        case WeaponType.Crossbow:
                            crossbow.transform.localPosition = Vector3.Lerp(CrossbowStartPosition, CrossbowEndPosition, lerpAmount);
                            crossbow.transform.localRotation = Quaternion.Euler(Vector3.Lerp(CrossbowStartRotation, CrossbowEndRotation, lerpAmount));
                            break;
                    }
                    break;
            }
        }
    }

    private void ProcessPlayerAction() {
        switch (playerSelectedAction.passive) {
            case PassiveEffect.Burn:
                return;
            case PassiveEffect.Heal:
                return;
            case PassiveEffect.Shield:
                return;
        }

        enemyHealth -= playerSelectedAction.actionIntensity;
    }

    private void ProcessEnemyAction() {
        switch (enemySelectedAction.passive) {
            case PassiveEffect.Burn:
                return;
            case PassiveEffect.Heal:
                return;
            case PassiveEffect.Shield:
                return;
        }

        playerHealth -= enemySelectedAction.actionIntensity;
    }

    private void UpdateGUI(){
        enemyHealthBar.SetBarFill(enemyHealth/enemy.health);
        healthBar.SetBarFill(playerHealth/playerHealthMax);
        actionBar.SetBarFill(playerTurnChance/playerTurnDefault);
    }

    void Update() {
        if (combatCanvas.enabled == false) {
            return;
        }

        localTime += Time.deltaTime;

        HandleCombatState(combatState);
        HandleGUI(playerGUI);
        HandleEnemyAnimation();
        UpdateGUI();

        if (enemyHealth <= 0) {
            PlayerWinCombat.Invoke();
            combatCanvas.enabled = false;
        } else if (playerHealth <= 0) {
            PlayerLoseCombat.Invoke();
            combatCanvas.enabled = false;
        }
    }

    void SetObjsActive(bool active, params GameObject[] objs) {
        foreach(GameObject obj in objs) {
            obj.SetActive(active);
        }
    }

    Enemy FindEnemy(string enemyName) {
        foreach(Enemy enemy in Enemies) {
            if (enemy.name == enemyName) {
                return enemy;
            }
        }
        Log($"{enemyName} not found in Enemies");
        return emptyEnemy;
    }
    
    Action FindAction(string actionName) {
        foreach(Action action in Actions) {
            if (action.name == actionName) {
                return action;
            }
        }
        Log($"{actionName} not found in Actions");
        return emptyAction;
    }

    void Log(string msg) {
        if (DebugMode) {
            Debug.Log(msg);
        }
    }
}
