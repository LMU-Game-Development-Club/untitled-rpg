using UnityEngine;

public class MiloLimbScriptTemplate : MonoBehaviour
{
    [Header("Limb Components")]
    public string limbName;
    public GameObject attack1;
    public GameObject attack2;
    public float limbHealth;
    public float limbMaxHealth;
    public bool isBroken;
    public int limbMaxCooldown;
    public int limbCooldown;
    public string limbDescription;
}
