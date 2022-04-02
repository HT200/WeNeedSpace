using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisions : MonoBehaviour
{
    public GameManager m_gameManager;
    public ScoreManager m_scoreManager;

    void Start()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check collision between a bullet and an enemy
        if (this.tag == "Bullet")
        {
            if (other.tag == "Enemy")
            {
                // Update list of enemies
                for (int i = 0; i < m_gameManager.enemyList.Count; i++)
                {
                    if(m_gameManager.enemyList[i].gameObject == other.gameObject)
                    {
                        print(m_gameManager.enemyList.Count);
                        m_gameManager.enemyList.RemoveAt(i);
                        print(m_gameManager.enemyList.Count);
                        break;
                    }
                }

                Destroy(other.gameObject);
                Destroy(this.gameObject);
                m_scoreManager.IncrementKill();
                m_scoreManager.UpdateScoreEnemyHit();
                /*
                enemyScript.UpdateHealth(-playerScript.damage);
                */
            }
            else if (other.tag == "Asteroid")
            {
                // Update list of asteroids
                for (int i = 0; i < m_gameManager.asteroidList.Count; i++)
                {
                    if(m_gameManager.asteroidList[i].gameObject == other.gameObject)
                    {
                        m_gameManager.asteroidList.RemoveAt(i);
                        break;
                    }
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
            if (other.tag == "Enemy" || other.tag == "Asteroid")
            {
                //This should cause an explosion, for now it means destroying the enemy
                Destroy(other.gameObject);
                // Destroy(this.gameObject);
            }
        }
        //Since detecting collision works both ways, we dont need to create reciprocal if statements for the enemy (all combinations are already handled)
    }

    private void Update()
    {
    }
}
