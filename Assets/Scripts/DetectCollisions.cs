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
        m_scoreManager = GameObject.Find("GameManager").GetComponent<ScoreManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check collision between a bullet and an enemy
        if (this.tag == "Bullet")
        {
            if (other.tag == "Enemy")
            {
                //This line isnt necessary, as the enemies all die in one hit currently so no health mechanics, no need for script

                //EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();

                Destroy(other.gameObject);
                Destroy(this.gameObject);
                m_scoreManager.IncrementKill();
                m_scoreManager.UpdateScoreEnemyHit();
                /*
                enemyScript.UpdateHealth(-playerScript.damage);
                */
            }
            else if (other.tag == "Bullet" || other.tag == "Asteroid")
            {
                Destroy(this.gameObject);
                Destroy(other.gameObject);
            }
        }
        else if (this.tag == "Player")
        {
            if (other.tag == "Enemy" || other.tag == "Asteroid")
            {
                //This should cause an explosion, for now it means destroying the enemy
                Destroy(other.gameObject);
                m_gameManager.SafeShutdown();            
            }
        }
        //Since detecting collision works both ways, we dont need to create reciprocal if statements for the enemy (all combinations are already handled)
    }
}
