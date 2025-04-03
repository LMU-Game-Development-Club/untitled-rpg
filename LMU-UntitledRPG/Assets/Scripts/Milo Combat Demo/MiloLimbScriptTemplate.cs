using UnityEngine;

public class MiloLimbScriptTemplate : MonoBehaviour
{
    [Header("Limb Components")]
    public string limbName;
    public GameObject[] attacks;
    public float limbHealth;
    public float limbMaxHealth;
    public bool isBroken;
    public int limbMaxCooldown;

    private int limbCooldown;
    public string limbDescription;
}
