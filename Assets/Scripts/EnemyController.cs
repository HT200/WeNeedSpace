using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { EASY, MEDIUM, HARD }

public class EnemyController : MonoBehaviour
{
    public GameManager gameManager;

    protected GameObject player;

    float health = 1.0f;

    // Can be true if this enemy has only been hit with critical hits
    bool perfectDestroy = true;

    // Multiplier for critical hits
    float critMult = 2.0f;

    // The center point (in world-space) at which this enemy will recieve critical damage
    Vector3 weakPoint;

    // This enemy's type, decided when spawned
    public EnemyType m_enemyType;

    protected Vector3 pos;
    protected Vector3 vel;
    protected Vector3 target;

    protected float speed = 6.0f;

    protected virtual void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = gameManager.player;

        // Generate random target in the direction of the player
        Vector3 position = transform.position;
        Vector3 halfToPlayer = position + (player.transform.position - position) / 2;
        target = halfToPlayer + Random.onUnitSphere * halfToPlayer.magnitude;

        // Send the enemy toward the target
        transform.forward = (target - position).normalized;

        vel = transform.forward * speed;
        pos = position;
    }

    protected virtual void Update()
    {
        float dt = Time.deltaTime;

        // Update the world-space location of this enemy's weakpoint as it moves
        // BoxCollider enemy = transform.GetComponent<BoxCollider>();
        // var center = enemy.center;
        // weakPoint = transform.TransformPoint(center.x, center.y, center.z - enemy.size.z / 2);

        vel = transform.forward * speed;
        pos += vel * dt;
        transform.position = pos;
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

        Destroy(gameObject);
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

    public void OnGUI()
    {
        GUI.color = Color.white;
        GUI.skin.box.fontSize = 15;
        GUI.skin.box.wordWrap = false;
    }
}
