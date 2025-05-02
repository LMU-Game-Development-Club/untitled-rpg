/**
    Name: Adi Roitburg
    Date of Creation: 2/23/2025
    Description: Properties of an enemy in battle
*/

using UnityEngine;
using UnityEngine.Timeline;

public class EnemyInCombat_ES : MonoBehaviour
{
    [Header("Enemy Attributes")]
    public ES_EnemyAttack[] attacks;
    public int numOfTurns;
    public float health;
    public string enemyName;
    public bool isDead = false;

    public void TakeDamage(float damageTaken) {
        health -= damageTaken;

        if(health <= 0) {
            isDead = true;
        }
    }
}

