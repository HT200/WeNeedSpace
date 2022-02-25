using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardEnemy : EnemyController
{
    // Is the enemy targeting the player
    bool seeking;

    public GameObject laserfire;
    float laserTimer;
    float laserCooldown = 1.0f;
    float maxStrafeDist = 20.0f;
    float maxFireDistance = 60.0f;

    protected override void Start()
    {
        base.Start();

        speed = 6.0f;
        seeking = false;
        laserTimer = laserCooldown;
    }

    protected override void Update()
    {
        float dt = Time.deltaTime;

        Vector3 playerPos = player.transform.position;

        // Seek the player after retreating from a strafe
        if (!seeking && Vector3.Distance(pos, target) < 1.0f)
        {
            seeking = true;

            target = playerPos;
            transform.forward = (target - pos).normalized;
        }

        // Seek the player during a strafe
        if (seeking)
        {
            if (Vector3.Distance(pos, target) < maxStrafeDist)
            {
                // The enemy is too close to the player, retreat to a random target behind it
                seeking = false;

                Vector3 toPlayer = pos + (playerPos - pos) / 2;
                target = -transform.forward * maxFireDistance + Random.onUnitSphere * maxFireDistance;

                transform.forward = (target - pos).normalized;
            }
            else
            {
                // Target the player during a strafe
                target = playerPos;
                transform.forward = (target - pos).normalized;

                // Fire every 1 second while strafing while within firing range
                if (Vector3.Distance(pos, target) < maxFireDistance && laserTimer <= 0.0f)
                {
                    GameObject laser = Instantiate(laserfire, pos + transform.forward * 3f, transform.rotation);
                    laser.GetComponent<Laser>().speed = 20.0f;
                    laserTimer = laserCooldown;
                }
                else if (laserTimer > 0.0f)
                {
                    laserTimer -= dt;
                }
            }
        }

        base.Update();
    }
}
