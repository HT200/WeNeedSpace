using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardEnemyBehavior : MonoBehaviour
{
    public GameObject player;
    PlayerController playerScript;

    public GameObject laserfire;
    float laserTimer;
    float laserCooldown = 1.0f;
    float maxStrafeDist = 20.0f;
    float maxFireDistance = 60.0f;

    Vector3 pos;
    Vector3 vel;
    Vector3 target;

    // Is the enemy targeting the player
    bool seeking;
    // // Is the enemy strafing
    // bool strafing;

    float speed = 6.0f;

    void Start()
    {
        // TODO: Replace with player prefab
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<PlayerController>();

        // Generate random target in the direction of the player
        Vector3 halfToPlayer = transform.position + (player.transform.position - transform.position) / 2;
        target = halfToPlayer + Random.onUnitSphere * halfToPlayer.magnitude;

        // Send the enemy toward the target
        transform.forward = (target - transform.position).normalized;
        seeking = false;

        vel = transform.forward * speed;
        pos = transform.position;

        laserTimer = laserCooldown;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // Seek the player after retreating from a strafe
        if (!seeking && Vector3.Distance(transform.position, target) < 1.0f)
        {
            seeking = true;

            target = player.transform.position;
            transform.forward = (target - transform.position).normalized;
        }

        // Seek the player during a strafe
        if (seeking)
        {
            if (Vector3.Distance(transform.position, target) < maxStrafeDist)
            {
                // The enemy is too close to the player, retreat to a random target behind it
                seeking = false;

                Vector3 toPlayer = transform.position + (player.transform.position - transform.position) / 2;
                target = -transform.forward * toPlayer.magnitude * 2 + Random.onUnitSphere * toPlayer.magnitude;

                transform.forward = (target - transform.position).normalized;
            }
            else
            {
                // Target the player during a strafe
                target = player.transform.position;
                transform.forward = (target - transform.position).normalized;

                // Fire every 1 second while strafing while within firing range
                if (Vector3.Distance(transform.position, target) < maxFireDistance && laserTimer <= 0.0f)
                {
                    GameObject temp = Instantiate(laserfire, transform.position + transform.forward * 1.5f, transform.rotation);
                    laserTimer = laserCooldown;
                }
                else if (laserTimer > 0.0f)
                {
                    laserTimer -= dt;
                }
            }
        }

        vel = transform.forward * speed;
        pos += vel * dt;
        transform.position = pos;
    }
}
