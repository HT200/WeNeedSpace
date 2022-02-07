using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisions : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject player;
    PlayerController playerScript;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerScript = player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Check collision between a bullet and an enemy
        if (this.tag == "Bullet")
        {
            if (other.tag == "Enemy"){
                EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();

                float dist = Vector3.Distance(enemyScript.GetWeakPoint(), transform.position - other.transform.position);

                if (dist <= 0.5f)
                {
                    // Critical Hit
                    gameManager.UpdateScore(gameManager.scoreEnemyHit, true);
                    enemyScript.UpdateHealth(-playerScript.damage, true);
                }
                else
                {
                    gameManager.UpdateScore(gameManager.scoreEnemyHit, false);
                    enemyScript.UpdateHealth(-playerScript.damage, false);
                }
            }
            if (other.tag == "Player")
            {
                //The player has been hit
                Destroy(other.gameObject);
            }
            if(other.tag == "Bullet")
            {
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }


        } else if (this.tag == "Player")
        {
            if(other.tag == "Enemy")
            {
                //This should cause an explosion, for now it means destroying both
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
        //Since detecting collision works both ways, we dont need to create reciprocal if statements for the enemy (all combinations are already handled)
    }
}
