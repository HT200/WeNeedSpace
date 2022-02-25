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

                float dist = Vector3.Distance(enemyScript.GetWeakPoint(), transform.position - other.transform.position);

                if (dist <= 0.5f)
                {
                    // Critical Hit
                    Destroy(other.gameObject);
                    Destroy(this.gameObject);
                    gameManager.IncrementKill();
                    gameManager.UpdateScore(gameManager.scoreEnemyHit, true);
                    //Currently working with 1 health enemies, we'll deal with this later
                    /*
                    enemyScript.UpdateHealth(-playerScript.damage, true);
                    */
                }
                else
                {
                    Destroy(other.gameObject);
                    Destroy(this.gameObject);
                    gameManager.IncrementKill();
                    gameManager.UpdateScore(gameManager.scoreEnemyHit, false);
                    /*
                    enemyScript.UpdateHealth(-playerScript.damage, false);
                    */
                }
            }
            else if (other.tag == "Bullet")
            {
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }else if(other.tag == "Wall")
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
            if (other.tag == "Wall")
            {
                this.gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }else if (this.tag == "Wall")
        {
            if(other.tag == "Player")
            {
                other.gameObject.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
        //Since detecting collision works both ways, we dont need to create reciprocal if statements for the enemy (all combinations are already handled)
    }

    private void Update()
    {
    }

    private void OnGUI()
    {
        GUI.color = Color.white;
        GUI.skin.box.fontSize = 15;
        GUI.skin.box.wordWrap = false;
    }
}
