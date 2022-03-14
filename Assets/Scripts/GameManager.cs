using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WaveState { IN_PROGRESS, COMPLETED }

public class GameManager : MonoBehaviour
{
    public GameObject player;

    int score;
    int totalkills;

    // Wave variables
    private int m_waveNumber = 0;
    private int m_waveWeight = 0;
    private WaveState m_waveState;

    // Timer variables
    private float m_spawnTimer;
    [SerializeField] private float m_timeBetweenSpawns = 1.5f;
    private float m_waveTimer;
    [SerializeField] private float m_timeBetweenWaves = 10f;
    [SerializeField] private float m_playTime = 0.0f;

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
    // A wave was cleared
    public int scoreWaveClear = 100;
    // TODO: Add more...

    //Combo for accurate shots
    int combo = 20;
    // TODO: Add more...

    // UI Element Variables
    [SerializeField] private Text m_scoreText;
    [SerializeField] private Text m_waveText;
    [SerializeField] private Text m_outOfBoundTop;
    [SerializeField] private Text m_outOfBoundBot;
    [SerializeField] private Text m_timer;
    [SerializeField] private Text m_waveTimeText;
    [SerializeField] private Text m_waveWarnText;
    [SerializeField] private Text m_gameOverText;

    Color boundColor;
    bool changingColor;
    public bool outOfBounds;

    void Start()
    {
        changingColor = true;
        boundColor = new Color(128, 0, 0);
        score = 0;
        totalkills = 0;

        NewWave();
    }

    void NewWave()
    {
        // Increase the wave variables
        m_waveNumber += 1;
        m_waveWeight = 10 * m_waveNumber;

        DetermineWaveEnemies();

        // Initialize the timers
        m_spawnTimer = m_timeBetweenSpawns;
        m_waveTimer = m_timeBetweenWaves;
        m_playTime = 0.0f;

        // Immediately start the first wave
        m_waveState = WaveState.IN_PROGRESS;
        m_waveText.text = "Wave: " + m_waveNumber;
        Debug.Log("Wave " + m_waveNumber + " started!");
        UpdateScore(0);
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
                NewWave();
            }
        }

        // Continue with the wave
        if(m_waveState == WaveState.IN_PROGRESS)
        {
            m_playTime += Time.deltaTime;
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
            SetWaveTimerWarning(true);
            // Decrease the timer by each frame duration
            m_waveTimer -= Time.deltaTime;
            m_waveTimeText.text = (Mathf.Round(m_waveTimer*100)/100).ToString();

            // Once the wave timer hits zero, start the next wave
            if(m_waveTimer <= 0)
            {
                TimeBonus();
                SetWaveTimerWarning(false);
                NewWave();
            }
        }
    }

    public void BoundWarning(float dtimer)
    {
        m_outOfBoundTop.gameObject.SetActive(true);
        m_outOfBoundBot.gameObject.SetActive(true);
        m_timer.gameObject.SetActive(true);
        m_timer.text = (Mathf.Round(dtimer*100)/100).ToString() + "s";
        if (changingColor)
        {
            boundColor.r -= 1;
        }
        else
        {
            boundColor.r += 1;
        }

        if(boundColor.r >= 255)
        {
            changingColor = true;
        }else if(boundColor.r <= 0)
        {
            changingColor = false;
        }
        m_outOfBoundTop.color = boundColor;
        m_outOfBoundBot.color = boundColor;

    }

    public void RemoveWarning()
    {
        m_outOfBoundBot.gameObject.SetActive(false);
        m_outOfBoundTop.gameObject.SetActive(false);
    }

    public void SetWaveTimerWarning(bool b)
    {
        m_waveTimeText.gameObject.SetActive(b);
        m_waveWarnText.gameObject.SetActive(b);
    }

    public void CheckWaveCompleted()
    {
        // If all enemies have been spawned and destroyed, then the wave has been completed
        if (m_numEasyEnemies != 0 || m_numMediumEnemies != 0 || m_numHardEnemies != 0 ||
            m_spawnPosition.childCount != 0) return;

        m_waveState = WaveState.COMPLETED;
        Debug.Log("Wave " + m_waveNumber + " completed!");
        score += scoreWaveClear;

        // Regenerate the player's Shield once a Wave is completed
        player.GetComponent<PlayerController>().RegenerateShield();
    }

    private void SpawnEnemy()
    {
        // Spawn the easy enemies first
        if (m_numEasyEnemies > 0)
        {
            GameObject easyEnemy = GameObject.Instantiate(m_easyEnemyPrefab, m_spawnPosition);
            easyEnemy.name = "EasyEnemy" + m_numEasyEnemies;
            easyEnemy.GetComponent<EnemyController>().m_enemyType = EnemyType.EASY;
            Debug.Log("Spawned " + easyEnemy.name);
            // Decrease the number of medium enemies left to spawn
            m_numEasyEnemies -= 1;
        }
        // Then spawn the medium enemies
        else if (m_numMediumEnemies > 0)
        {
            GameObject mediumEnemy = GameObject.Instantiate(m_mediumEnemyPrefab, m_spawnPosition);
            mediumEnemy.name = "MediumEnemy" + m_numMediumEnemies;
            mediumEnemy.GetComponent<EnemyController>().m_enemyType = EnemyType.MEDIUM;
            Debug.Log("Spawned " + mediumEnemy.name);
            // Decrease the number of medium enemies left to spawn
            m_numMediumEnemies -= 1;
        }
        // Then spawn the hard enemies
        else if (m_numHardEnemies > 0)
        {
            GameObject hardEnemy = GameObject.Instantiate(m_hardEnemyPrefab, m_spawnPosition);
            hardEnemy.name = "HardEnemy" + m_numHardEnemies;
            hardEnemy.GetComponent<EnemyController>().m_enemyType = EnemyType.HARD;
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
    public void UpdateScore(int num)
    {
        // Apply multipliers
        num += combo;

        // Log the Score update
        Debug.Log("Updating Score from " + score + " to " + (score + num));

        // Add to the player's score
        score += num;
        // Update the UI
        m_scoreText.text = "Score: " + score;
    }

    public void TimeBonus()
    {
        if(m_playTime < 60.0f)
        {
            score += 300;
        }
        else if(m_playTime > 180.0f)
        {
            score += (int)(300 - 2.5 * m_playTime);
        }
    }

    /// <summary>
    /// Get the player's current combo multiplier
    /// </summary>
    /// <returns>The player's current combo multiplier</returns>
    public int GetCombo()
    {
        return combo;
    }

    /// <summary>
    /// Set the player's current combo multiplier
    /// </summary>
    public void SetCombo(int newCombo)
    {
        combo = newCombo;
    }

    public void IncrementKill()
    {
        totalkills++;
        combo += 12;
    }

    public void DecrementCombo()
    {
        if (combo > 1)
        {
            combo-=2;
        }
    }

    public void SafeShutdown()
    {
        //Destroy the enemies
        TestDestroyCurrentWave();
        //Freeze the Player
        player.GetComponent<PlayerController>().freeze = true;
        //Display that you've lost
        m_gameOverText.gameObject.SetActive(true);
        //Store score (WIP)
    }
}
