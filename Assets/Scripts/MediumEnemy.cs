using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumEnemy : EnemyController
{
    // Is the enemy targeting the player
    bool seeking;

    protected override void Start()
    {
        base.Start();

        speed = 6.0f;
        seeking = false;
    }

    protected override void Update()
    {
        Vector3 playerPos = player.transform.position;

        // Do not target the player until the enemy has reached its first target
        if (!seeking && Vector3.Distance(pos, target) < 1.0f)
        {
            seeking = true;
        }

        // Continually re-target the player
        if (seeking)
        {
            target = playerPos;
            transform.forward = (target - pos).normalized;
        }
        
        base.Update();
    }
}
