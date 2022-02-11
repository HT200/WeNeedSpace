using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;

    int score;

    // Wave variables
    private int m_waveNumber = 0;
    private int m_waveWeight = 0;
    private WaveState m_waveState;

    // Timer variables
    private float m_spawnTimer;
    [SerializeField] private float m_timeBetweenSpawns = 1.5f;
    private float m_waveTimer;
    [SerializeField] private float m_timeBetweenWaves = 10f;

    // Enemy variables
    private int m_numEasyEnemies;
    [SerializeField] private int m_easyEnemyWeight;
    [SerializeField] private GameObject m_easyEnemyPrefab;
    private int m_numMediumEnemies;
    [SerializeField] private int m_mediumEnemyWeight;
    [SerializeField] private GameObject m_mediumEnemyPrefab;
    private int m_numHardEnemies;
    [SerializeField] private int m_hardEnemyWeight;
    [SerializeField] private GameObject m_hardEnemyPrefab;

    // Mothership spawn location
    [SerializeField] private Transform m_spawnPosition;

    // Score Types
    // An enemy was hit
    public int scoreEnemyHit = 5;
    // An enemy was destroyed with only critical hits
    public int scorePerfectDestroy = 50;
    // A wave was cleared
    public int scoreWaveClear = 100;
    // TODO: Add more...

    // Multipliers
    // How many enemies killed without getting hit
    int comboMult = 1;
    // Critically damaged an enemy
    int critMult = 2;
    // TODO: Add more...

    void Start()
    {
        score = 0;

        // Increase the wave variables
        m_waveNumber += 1;
        m_waveWeight = 10 * m_waveNumber;
        
        DetermineWaveEnemies();

        // Initialize the timers
        m_spawnTimer = m_timeBetweenSpawns;
        m_waveTimer = m_timeBetweenWaves;

        // Immediately start the first wave
        m_waveState = WaveState.IN_PROGRESS;
        Debug.Log("Wave " + m_waveNumber + " started!");
    }

    void Update()
    {
        // Use only for development purposes. Pressing Enter should destroy all enemies to complete the current wave
        if (Input.GetKey(KeyCode.Return))
        {
            // If the wave is in progress, destroy the current wave
            if (m_waveState == WaveState.IN_PROGRESS)
            {
                TestDestroyCurrentWave();
            }
            // Otherwise we immediately restart the next wave
            else if (m_waveState == WaveState.COMPLETED)
            {
                Start();
            }
        }

        // Continue with the wave
        if(m_waveState == WaveState.IN_PROGRESS)
        {
            // Decrease the timer buy each frame duration
            m_spawnTimer -= Time.deltaTime;

            // Once the spawn timer equals zero, spawn the next enemy and reset the spawn timer
            if(m_spawnTimer <= 0)
            {
                SpawnEnemy();
            }

            // Checks and handles wave completion criteria
            CheckWaveCompleted();
        }        
        // If a wave has been completed, we are inbetween waves
        else if(m_waveState == WaveState.COMPLETED)
        {
            // Decrease the timer by each frame duration
            m_waveTimer -= Time.deltaTime;

            // Once the wave timer hits zero, start the next wave
            if(m_waveTimer <= 0)
            {
                Start();
            }
        }

        // OnGUI();
    }

    public void CheckWaveCompleted()
    {
        // If all enemies have been spawned and destroyed, then the wave has been completed
        if (m_numEasyEnemies == 0 &&
            m_numMediumEnemies == 0 &&
            m_numHardEnemies == 0 &&
            m_spawnPosition.childCount == 0)
        {
            m_waveState = WaveState.COMPLETED;
            Debug.Log("Wave " + m_waveNumber + " completed!");
        }
    }

    private void SpawnEnemy()
    {
        // Spawn the easy enemies first
        if (m_numEasyEnemies > 0)
        {
            GameObject easyEnemy = GameObject.Instantiate(m_easyEnemyPrefab, m_spawnPosition);
            easyEnemy.name = "EasyEnemy" + m_numEasyEnemies;
            Debug.Log("Spawned " + easyEnemy.name);
            // Decrease the number of medium enemies left to spawn
            m_numEasyEnemies -= 1;
        }
        // Then spawn the medium enemies
        else if (m_numMediumEnemies > 0)
        {
            GameObject mediumEnemy = GameObject.Instantiate(m_mediumEnemyPrefab, m_spawnPosition);
            mediumEnemy.name = "MediumEnemy" + m_numMediumEnemies;
            Debug.Log("Spawned " + mediumEnemy.name);
            // Decrease the number of medium enemies left to spawn
            m_numMediumEnemies -= 1;
        }
        // Then spawn the hard enemies
        else if (m_numHardEnemies > 0)
        {
            GameObject hardEnemy = GameObject.Instantiate(m_hardEnemyPrefab, m_spawnPosition);
            hardEnemy.name = "HardEnemy" + m_numHardEnemies;
            Debug.Log("Spawned " + hardEnemy.name);
            // Decrease the number of hard enemies left to spawn
            m_numHardEnemies -= 1;
        }

        // Reset the spawn timer
        m_spawnTimer = m_timeBetweenSpawns;
    }

    /// <summary>
    /// Determine how many of each enemy type will be spawned in the next wave.
    /// This is based on the weight of the incoming wave.
    /// </summary>
    private void DetermineWaveEnemies()
    {
        // Generate a random number of hard enemies between 0 and the max number possible given the current wave weight
        m_numHardEnemies = Random.Range(0, (int)(m_waveWeight / m_hardEnemyWeight));
        // Subtract the total enemy weight from the wave weight
        m_waveWeight -= (m_numHardEnemies * m_hardEnemyWeight);

        // Generate a random number of medium enemies between 0 and the max number possible given the current wave weight
        m_numMediumEnemies = Random.Range(0, (int)(m_waveWeight / m_mediumEnemyWeight));
        // Subtract the total enemy weight from the wave weight
        m_waveWeight -= (m_numMediumEnemies * m_mediumEnemyWeight);

        // The rest of the wave is filled with easy enemies
        m_numEasyEnemies = m_waveWeight / m_easyEnemyWeight;
        // Subtract the total enemy weight from the wave weight
        m_waveWeight -= (m_numEasyEnemies * m_easyEnemyWeight);
    }

    void TestDestroyCurrentWave()
    {
        foreach(Transform childTransform in m_spawnPosition)
        {
            Destroy(childTransform.gameObject);
        }
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
