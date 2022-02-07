using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject player;
    PlayerController playerScript;

    float health = 100.0f;

    // Can be true if this enemy has only been hit with critical hits
    bool perfectDestroy = true;

    // Multiplier for critical hits
    float critMult = 2.0f;

    // The center point (in world-space) at which this enemy will recieve critical damage
    Vector3 weakPoint;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerScript = player.GetComponent<PlayerController>();
    }

    void Update()
    {
        // Update the world-space location of this enemy's weakpoint as it moves
        BoxCollider enemy = transform.GetComponent<BoxCollider>();
        weakPoint = transform.TransformPoint(enemy.center.x, enemy.center.y, enemy.center.z - enemy.size.z / 2);
    }

    /// <summary>
    /// Destroy this enemy
    /// </summary>
    void DestroyEnemy()
    {
        // TODO
        print("Enemy Destroyed");

        // Apply extra points for a perfect destroy
        if (perfectDestroy)
        {
            gameManager.UpdateScore(gameManager.scorePerfectDestroy, false);
        }

        // Add to the player's combo
        gameManager.SetCombo(gameManager.GetCombo() + 1);
    }

    /// <summary>
    /// Get this enemy's health
    /// </summary>
    /// <returns>This enemy's current health</returns>
    public float GetHealth()
    {
        return health;
    }
    /// <summary>
    /// Update this enemy's health
    /// </summary>
    public void UpdateHealth(float num, bool crit)
    {
        if (crit)
        {
            // Critical hit, apply multiplier
            num *= critMult;
        }
        else
        {
            perfectDestroy = false;
        }

        health += num;
        print("Health: " + health);

        if (health <= 0)
        {
            DestroyEnemy();
        }
    }
    /// <summary>
    /// Get this enemy's weakpoint
    /// </summary>
    /// <returns>This enemy's weakpoint in world-space</returns>
    public Vector3 GetWeakPoint()
    {
        return weakPoint;
    }
}
