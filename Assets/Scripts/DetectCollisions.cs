using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisions : MonoBehaviour
{
    public GameManager gameManager;

    PlayerController playerScript;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerScript = gameManager.player.GetComponent<PlayerController>();
    }
    void OnTriggerEnter(Collider other)
    {
        // Check collision between a bullet and an enemy
        if (this.tag == "Bullet")
        {
            if (other.tag == "Enemy")
            {
                EnemyController enemyScript = other.gameObject.GetComponent<EnemyController>();

                Destroy(other.gameObject);
                Destroy(this.gameObject);
                gameManager.IncrementKill();
                gameManager.UpdateScore(gameManager.scoreEnemyHit);
                /*
                enemyScript.UpdateHealth(-playerScript.damage);
                */
            }
            if (other.tag == "Player")
            {
                //The player has been hit
                Destroy(other.gameObject);
            }
            if (other.tag == "Bullet")
            {
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
        else if (this.tag == "Player")
        {
            if (other.tag == "Enemy")
            {
                //This should cause an explosion, for now it means destroying both
                Destroy(other.gameObject);
                // Destroy(this.gameObject);
            }
        }
        else if (this.tag == "Player")
        {
            if (other.tag == "Wall")
            {
                this.gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
        //Since detecting collision works both ways, we dont need to create reciprocal if statements for the enemy (all combinations are already handled)
    }

    private void Update()
    {
    }
}
