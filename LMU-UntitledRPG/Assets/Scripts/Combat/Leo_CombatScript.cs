using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Leo_CombatScript : MonoBehaviour
{
    enum CombatState {
        PlayerMove,
        PlayerAnimation,
        EnemyMove,
        EnemyAnimation
    }

    enum CombatMove {
        None,
        Attack,
        Counter,
        LightAtk,
        HeavyAtk
    }

    private CombatState combatState;
    private bool combatStateChanged;
    private CombatMove combatMove;
    private bool combatMoveChanged;

    public GameObject hitEffectUIPrefab;

    private GameObject AttackButton;
    private GameObject CounterButton;
    private GameObject LightAtkButton;
    private GameObject HeavyAtkButton;

    private float localTime;
    private float playerAnimationSpeed = 1;
    private float playerAnimationEndTick;
    private float enemyMoveSpeed = 2;
    private float enemyMoveEndTick;
    private float enemyAnimationSpeed = 1;
    private float enemyAnimationEndTick;

    private List<GameObject> objectsToDumpAfterTransition;

    void Start() {
        AttackButton = transform.Find("AttackButton").gameObject;
        CounterButton = transform.Find("CounterButton").gameObject;
        LightAtkButton = transform.Find("LightAtkButton").gameObject;
        HeavyAtkButton = transform.Find("HeavyAtkButton").gameObject;

        combatState = CombatState.PlayerMove;
        combatStateChanged = true;
        combatMove = CombatMove.None;
        combatMoveChanged = true;

        localTime = 0;

        objectsToDumpAfterTransition = new List<GameObject>();
    }

    private void TransitionCombatState(CombatState newCombatState) {
        foreach ( GameObject obj in objectsToDumpAfterTransition) {
            Destroy(obj);
        }
        objectsToDumpAfterTransition = new List<GameObject>();

        combatState = newCombatState;
        combatStateChanged = true;
    }

    private void TransitionCombatMove(CombatMove newCombatMove) {
        combatMove = newCombatMove;
        combatMoveChanged = true;
    }

    void Update() {
        localTime += Time.deltaTime;

        if (combatStateChanged) {
            Debug.Log($"New combatState: {combatState}");
            switch (combatState) {
                case CombatState.PlayerMove:
                    TransitionCombatMove(CombatMove.None);
                    break;
                case CombatState.PlayerAnimation:
                    AttackButton.SetActive(false);
                    CounterButton.SetActive(false);
                    LightAtkButton.SetActive(false);
                    HeavyAtkButton.SetActive(false);
                    playerAnimationEndTick = localTime + playerAnimationSpeed;

                    //Player Animation code here

                    if (combatMove != CombatMove.Counter) {
                        GameObject playerHit = Instantiate(hitEffectUIPrefab);
                        playerHit.transform.parent = transform;
                        playerHit.GetComponent<RectTransform>().localPosition = new Vector3 (493, 384, 0);
                        objectsToDumpAfterTransition.Add(playerHit);
                    }
                    break;
                case CombatState.EnemyMove:
                    enemyMoveEndTick = localTime + enemyMoveSpeed;
                    break;
                case CombatState.EnemyAnimation:
                    enemyAnimationEndTick = localTime + enemyAnimationSpeed;

                    //Enemy Animation code here
                    GameObject enemyHit = Instantiate(hitEffectUIPrefab);
                    enemyHit.transform.parent = transform;
                    enemyHit.GetComponent<RectTransform>().localPosition = new Vector3(-513, 252, 0);
                    objectsToDumpAfterTransition.Add(enemyHit);
                    break;
            }
            combatStateChanged = false;
        } else {
            switch (combatState) {
                case CombatState.PlayerMove:
                    break;
                case CombatState.PlayerAnimation:
                    if (localTime >= playerAnimationEndTick) {
                        TransitionCombatState(CombatState.EnemyMove);
                    }
                    break;
                case CombatState.EnemyMove:
                    if (localTime >= enemyMoveEndTick) {
                        TransitionCombatState(CombatState.EnemyAnimation);
                    }
                    break;
                case CombatState.EnemyAnimation:
                    if (localTime >= enemyAnimationEndTick) {
                        TransitionCombatState(CombatState.PlayerMove);
                    }
                    break;
            }
        }

        if (combatMoveChanged && combatState == CombatState.PlayerMove) {
            switch (combatMove) {
                case CombatMove.None:
                    AttackButton.SetActive(true);
                    CounterButton.SetActive(true);
                    break;
                case CombatMove.Attack:
                    AttackButton.SetActive(false);
                    CounterButton.SetActive(false);
                    LightAtkButton.SetActive(true);
                    HeavyAtkButton.SetActive(true);
                    break;
                case CombatMove.Counter:
                    TransitionCombatState(CombatState.PlayerAnimation);
                    break;
                case CombatMove.LightAtk:
                    TransitionCombatState(CombatState.PlayerAnimation);
                    break;
                case CombatMove.HeavyAtk:
                    TransitionCombatState(CombatState.PlayerAnimation);
                    break;
            }
            combatMoveChanged = false;
        }
    }
    

    public void AttackButtonPress() {
        TransitionCombatMove(CombatMove.Attack);
    }
    
    public void CounterButtonPress() {
        TransitionCombatMove(CombatMove.Counter);
    }

    public void LightAtkButtonPress() {
        TransitionCombatMove(CombatMove.LightAtk);        
    }

    public void HeavyAtkButtonPress() {
        TransitionCombatMove(CombatMove.HeavyAtk);
    }
}
