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
        if (this.tag == "Bullet" && other.tag == "Enemy")
        {
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
    }
}
