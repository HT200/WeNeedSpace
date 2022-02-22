using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyEnemy : EnemyController
{
    protected override void Start()
    {
        base.Start();

        speed = 6.0f;
    }

    protected override void Update()
    {
        Vector3 playerPos = player.transform.position;

        // If this enemy has reached it's previous target, re-target the player
        if (Vector3.Distance(pos, target) < 1.0f)
        {
            target = playerPos;
            transform.forward = (target - pos).normalized;
        }

        base.Update();
    }
}
