using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { EASY, MEDIUM, HARD }

public class EnemyController : MonoBehaviour
{
    public GameManager gameManager;
    protected GameObject player;

    float health = 1.0f;
    protected float speed = 6.0f;

    // This enemy's type, decided when spawned
    public EnemyType m_enemyType;

    protected Vector3 pos;
    protected Vector3 vel;
    protected Vector3 target;

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

        vel = transform.forward * speed;
        pos += vel * dt;
        transform.position = pos;
    }

    /// <summary>
    /// Destroy this enemy
    /// </summary>
    void DestroyEnemy()
    {
        print("Enemy Destroyed");

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
    public void UpdateHealth(float num)
    {
        health += num;
        print("Health: " + health);

        if (health <= 0)
        {
            DestroyEnemy();
        }
    }
}
