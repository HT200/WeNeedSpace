using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisions : MonoBehaviour
{
    public GameManager m_gameManager;
    public ScoreManager m_scoreManager;
    public MiniMapController m_miniMapController;

    void Start()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        m_miniMapController = GameObject.Find("MiniMap").GetComponent<MiniMapController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check collision between a bullet and an enemy
        if (this.tag == "Bullet")
        {
            if (other.tag == "Enemy")
            {
                // Update list of enemies
                m_gameManager.UpdateEnemyList(other.gameObject);

                Destroy(other.gameObject);
                Destroy(this.gameObject);
                m_scoreManager.IncrementKill();
                m_scoreManager.UpdateScoreEnemyHit();
            }
            else if (other.tag == "Asteroid")
            {
                // Update list of asteroids
                m_gameManager.UpdateAsteroidList(other.gameObject);

                // Spawn a powerup if this asteroid has one
                AsteroidController script = other.GetComponent<AsteroidController>();
                if (script.m_hasPowerup)
                {
                    script.SpawnPowerup();
                }

                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
            else if (other.tag == "Bullet")
            {
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
        else if (this.tag == "Player")
        {
            if (other.tag == "Enemy")
            {
                // Update list of enemies
                m_gameManager.UpdateEnemyList(other.gameObject);

                //This should cause an explosion, for now it means destroying the enemy
                m_gameManager.player.GetComponent<PlayerController>().DamagePlayer();
                // m_miniMapController.DamageIndicator(other.transform.position);
                Destroy(other.gameObject);
            }
            if (other.tag == "Bullet")
            {
                if (!other.gameObject.GetComponent<Laser>().pBullet)
                {
                    m_gameManager.player.GetComponent<PlayerController>().DamagePlayer();
                    // m_miniMapController.DamageIndicator(other.transform.position);
                    Destroy(other.gameObject);
                }
            }
            else if (other.tag == "Asteroid")
            {
                // Update list of asteroids
                m_gameManager.UpdateAsteroidList(other.gameObject);

                //This should cause an explosion, for now it means destroying the enemy
                m_gameManager.player.GetComponent<PlayerController>().DamagePlayer();
                // m_miniMapController.DamageIndicator(other.transform.position);
                Destroy(other.gameObject);
            }
            else if (other.tag == "Powerup")
            {
                StartCoroutine(other.GetComponent<PowerupController>().Apply());
                Destroy(other.gameObject);
            }
        }
        //Since detecting collision works both ways, we dont need to create reciprocal if statements for the enemy (all combinations are already handled)
    }
}
