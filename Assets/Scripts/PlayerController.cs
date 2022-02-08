using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager gameManager;

    // The player's health and damage output
    public float health = 100.0f;
    public float damage = 5.0f;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {

    }

    /// <summary>
    /// Update the player's health
    /// </summary>
    public void UpdateHealth(float num)
    {
        if (num < 0)
        {
            // Player has taken damage, reset combo
            gameManager.SetCombo(1);
        }

        health += num;
        print(num + " Health");
    }

    /// <summary>
    /// Update the player's damage output
    /// </summary>
    public void UpdateDamage(float num)
    {
        // TODO: Upgrade weapons for more damage?
        damage += num;
        print("Damage: " + damage);
    }
}
