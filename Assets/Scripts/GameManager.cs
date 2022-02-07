using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;

    int score;

    // Score Types:
    // An enemy was hit
    public int scoreEnemyHit = 5;
    // An enemy was destroyed with only critical hits
    public int scorePerfectDestroy = 50;
    // A wave was cleared
    public int scoreWaveClear = 100;
    // TODO: Add more...

    // Multipliers:
    // How many enemies killed without getting hit
    int comboMult = 1;
    // Critically damaged an enemy
    int critMult = 2;
    // TODO: Add more...

    void Start()
    {
        score = 0;
    }

    void Update()
    {
        OnGUI();
    }

    /// <summary>
    /// Update the player's score
    /// </summary>
    public void UpdateScore(int num, bool crit)
    {
        // Apply multipliers
        num *= comboMult;
        if (crit)
        {
            num *= critMult;
        }

        // Add to the player's score
        score += num;
        print("Score: " + score);
    }

    /// <summary>
    /// Get the player's current combo multiplier
    /// </summary>
    /// <returns>The player's current combo multiplier</returns>
    public int GetCombo()
    {
        return comboMult;
    }
    /// <summary>
    /// Set the player's current combo multiplier
    /// </summary>
    public void SetCombo(int newCombo)
    {
        comboMult = newCombo;
    }

    public void OnGUI()
    {
        GUI.color = Color.white;
        GUI.skin.box.fontSize = 15;
        GUI.skin.box.wordWrap = false;

        GUI.Box(new Rect(0, 0, 300, 30), "Current score: " + score);
    }
}
