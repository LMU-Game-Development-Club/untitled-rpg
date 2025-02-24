using UnityEngine;

public class MiloAttackScriptTemplate : MonoBehaviour
{
    [Header("Attack Components")]
    public string attackName;
    public float damage;
    public string element;
    public float elementDamage;
    public string status;
    public float statusBuildUp;
    // turn decay is the percentage amount added to the enemy turn meter after the attack is used
    public float turnDecay;
    public string attackDescription;
}
