using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyEnemyBehavior : MonoBehaviour
{
    public GameObject player;
    PlayerController playerScript;

    Vector3 pos;
    Vector3 vel;
    Vector3 target;

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

        vel = transform.forward * speed;
        pos = transform.position;
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // If this enemy has reached it's previous target, re-target the player
        if (Vector3.Distance(transform.position, target) < 1.0f)
        {
            target = player.transform.position;
            transform.forward = (target - transform.position).normalized;
        }

        vel = transform.forward * speed;
        pos += vel * dt;
        transform.position = pos;
    }
}
