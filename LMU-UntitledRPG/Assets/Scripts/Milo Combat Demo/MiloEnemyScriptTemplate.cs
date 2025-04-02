using UnityEngine;

public class MiloEnemyScriptTemplate : MonoBehaviour
{
    [Header("Enemy Components")]
    public string enemyName;
    public float health;
    public float maxHealth;

    public GameObject[] attacks;
    
    public int attacksPerTurn;
    public string[] activeStatusEffects;
    public int[] statusBuildUps;
    public string enemyDescription;
}
