using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WaveState { IN_PROGRESS, COMPLETED, OVER }

public class GameManager : MonoBehaviour
{
    public GameObject player;

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

    // Mothership spawn location
    [SerializeField] private Transform m_spawnPosition;
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

    // Score Manager
    [SerializeField] private ScoreManager m_scoreManager;

    // Asteroid variables
    [SerializeField] private GameObject m_AsteroidBasePrefab;
    private int m_maxAsteroids = 20;

    // UI Element Variables
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
        for(int i = 0; i < m_maxAsteroids; i += 1)
        {
            GameObject.Instantiate(m_AsteroidBasePrefab, Vector3.zero, Quaternion.identity);
        }

        changingColor = true;
        boundColor = new Color(128, 0, 0);

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
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightControl))
        {
            SafeShutdown();
        }
        // Use only for development purposes. Pressing Enter should destroy all enemies to complete the current wave
        if (Input.GetKeyDown(KeyCode.Return))
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
            else if (m_waveState == WaveState.OVER)
            {
                //This block needs a scene or state transition to avoid a player writing to file multiple times
                m_scoreManager.ReadFromFile();
                m_scoreManager.WriteToFile();
            }
        }

        // Continue with the wave
        if (m_waveState == WaveState.IN_PROGRESS)
        {
            m_playTime += Time.deltaTime;
            // Decrease the timer by each frame duration
            m_spawnTimer -= Time.deltaTime;

            // Once the spawn timer equals zero, spawn the next enemy and reset the spawn timer
            if (m_spawnTimer <= 0)
            {
                SpawnEnemy();
            }

            // Checks and handles wave completion criteria
            CheckWaveCompleted();
        }
        // If a wave has been completed, we are inbetween waves
        //Note: we should change all these else ifs to a single switch statement
        //since an enum cant be two states at once making them explicitly mutually exclusive is unnecessary
        else if (m_waveState == WaveState.COMPLETED)
        {
            SetWaveTimerWarning(true);
            // Decrease the timer by each frame duration
            m_waveTimer -= Time.deltaTime;
            m_waveTimeText.text = (Mathf.Round(m_waveTimer * 100) / 100).ToString();

            // Once the wave timer hits zero, start the next wave
            if (m_waveTimer <= 0)
            {
                m_scoreManager.TimeBonus(m_playTime);
                SetWaveTimerWarning(false);
                NewWave();
            }
        }
        else if (m_waveState == WaveState.OVER)
        {
            m_scoreManager.GetName();
        }
    }

    public void BoundWarning(float dtimer)
    {
        if (m_waveState != WaveState.OVER)
        {
            m_outOfBoundTop.gameObject.SetActive(true);
            m_outOfBoundBot.gameObject.SetActive(true);
            m_timer.gameObject.SetActive(true);
            m_timer.text = (Mathf.Round(dtimer * 100) / 100).ToString() + "s";

            if (changingColor)
            {
                boundColor.r -= 1;
            }
            else
            {
                boundColor.r += 1;
            }

            if (boundColor.r >= 255)
            {
                changingColor = true;
            }
            else if (boundColor.r <= 0)
            {
                changingColor = false;
            }
            m_outOfBoundTop.color = boundColor;
            m_outOfBoundBot.color = boundColor;
        }
    }

    public void RemoveWarning()
    {
        m_outOfBoundBot.gameObject.SetActive(false);
        m_outOfBoundTop.gameObject.SetActive(false);
        m_timer.gameObject.SetActive(false);
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
        m_scoreManager.UpdateScoreWaveCleared();

        // Regenerate the player's Shield once a Wave is completed
        player.GetComponent<PlayerController>().RegenerateShield();
    }

    /// <summary>
    /// Spawn the enemies for the current wave. This is called in Update, and one enemy
    /// is spawned at a time, first easy, then medium, then hard.
    /// </summary>
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
        foreach (Transform childTransform in m_spawnPosition)
        {
            Destroy(childTransform.gameObject);
        }
    }

    /// <summary>
    /// Safely shutsdown the game
    /// </summary>
    public void SafeShutdown()
    {
        //While this hardly a wavestate its much easier to attach this to an enum so it essentially turns off the other two's update methods in the process
        m_waveState = WaveState.OVER;

        //Freeze time (this only works on things that run on dt, so player UI must be disabled seperately so you can't rotate)
        Time.timeScale = 0f;
        //Display that you've lost, and show name entry
        m_gameOverText.gameObject.SetActive(true);
        m_scoreManager.DisplayNameText();
        //Note: If you were to die by going out of bounds, this could be done easier through the player script
        //but since the player can die multiple ways we need one method
        player.GetComponent<PlayerController>().GetPlayerUI().enabled = false;
        player.GetComponent<PlayerController>().GetPlayerUI().gameObject.SetActive(false);

        //Store score With associated name (max 10 characters)(WIP)

        //Return to main menu (TBA)
    }
}
