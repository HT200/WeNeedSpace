using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum WaveState { IN_PROGRESS, COMPLETED, OVER }

public class GameManager : MonoBehaviour
{
    private Vector3 ogSpaceIndicator;
    private Vector3 endSpaceIndicator;

    public GameObject player;

    // Asteroid variables
    [SerializeField] private GameObject m_AsteroidBasePrefab;
    int m_maxAsteroids = 20;

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

    //For altering the name
    int nameIndex;
    const string alphabet = "_ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()+=;\"~`<>?";

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
    [SerializeField] private Text m_nameText;
    [SerializeField] private Text m_spaceIndicator;

    //for storing score/name pairs in I/O
    List<string> scores;


    Color boundColor;
    bool changingColor;
    public bool outOfBounds;

    void Start()
    {
        ogSpaceIndicator = m_spaceIndicator.gameObject.transform.position;
        endSpaceIndicator = new Vector3(ogSpaceIndicator.x + 250, ogSpaceIndicator.y, ogSpaceIndicator.z);
        for(int i = 0; i < m_maxAsteroids; i += 1)
        {
            GameObject.Instantiate(m_AsteroidBasePrefab, Vector3.zero, Quaternion.identity);
        }
        nameIndex = 0;
        scores = new List<string>();
        m_nameText.text = "__________";
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
                ReadFromFile();
                WriteToFile();
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
                TimeBonus();
                SetWaveTimerWarning(false);
                NewWave();
            }
        }
        else if (m_waveState == WaveState.OVER)
        {


            if (Input.anyKey)
            {
                //Note: Nameindex can range from 0-9

                //We need to use a temp string because indexing a string is read only, so we can't alter each character individually
                string alteredString = "";

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (nameIndex != 9)
                    {
                        m_spaceIndicator.gameObject.transform.position = new Vector3(
                        m_spaceIndicator.gameObject.transform.position.x + 25,
                        m_spaceIndicator.gameObject.transform.position.y,
                        m_spaceIndicator.gameObject.transform.position.z
                        );
                    }
                    else
                    {
                        m_spaceIndicator.gameObject.transform.position = ogSpaceIndicator;
                    }
                    nameIndex = (nameIndex + 1) % 10;
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    m_spaceIndicator.gameObject.transform.position = new Vector3(
                    m_spaceIndicator.gameObject.transform.position.x - 25,
                    m_spaceIndicator.gameObject.transform.position.y,
                    m_spaceIndicator.gameObject.transform.position.z
                    );
                    //For whatever reason,  modulus doesn't work with negative numbers (even though it should?). I've replicated the effect with this
                    //I'll need to do the same for the downarrow logic
                    if (nameIndex == 0)
                    {
                        nameIndex = 9;
                        m_spaceIndicator.gameObject.transform.position = endSpaceIndicator;
                    }
                    else
                    {
                        nameIndex--;
                    }
                }
                Debug.Log("name index is: " + nameIndex);
                //Note: Name index is in the length parameter of the substring, this is only ok because its starting at index 0, subtring(int, int) is not giving two indices and taking everything between them, one is the start the other is the length
                //This adds everything UP TO the space your altering to the temp string
                //if your altering the first space, theres no previous text to copy

                //adding the start
                if (nameIndex != 0)
                {
                    alteredString += m_nameText.text.ToString().Substring(0, nameIndex);
                }

                //Adding the current character
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    alteredString += alphabet[(alphabet.IndexOf(m_nameText.text.ToString()[nameIndex]) + 1) % alphabet.Length];

                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    if (m_nameText.text.ToString()[nameIndex] == alphabet[0])
                    {
                        alteredString += alphabet[alphabet.Length - 1];
                    }
                    else
                    {
                        alteredString += alphabet[(alphabet.IndexOf(m_nameText.text.ToString()[nameIndex]) - 1)];
                    }
                }
                else
                {
                    alteredString += m_nameText.text.ToString()[nameIndex];
                }


                //Adding the end
                if (nameIndex != 9)
                {
                    alteredString += m_nameText.text.ToString().Substring(nameIndex + 1, 9 - nameIndex);
                }
                //Rewrite
                m_nameText.text = alteredString;
            }
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
        foreach (Transform childTransform in m_spawnPosition)
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
    /// <summary>
    /// Applies a bonus based on how much time it took for the player to destroy the wave
    /// </summary>
    public void TimeBonus()
    {
        //If the player defeated the wave in under a minute, flat 300 bonus
        if (m_playTime < 60.0f)
        {
            score += 300;
        }
        //If between 1-3 minutes, use variable, its should be 300 at 60, 0 at 180
        else if (m_playTime < 180.0f)
        {
            score += (int)(300 - 2.5 * (m_playTime - 60.0f));
        }
        //Otherwise give 0 points
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
            combo -= 2;
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
        m_spaceIndicator.gameObject.SetActive(true);
        m_gameOverText.gameObject.SetActive(true);
        m_nameText.gameObject.SetActive(true);
        //Note: If you were to die by going out of bounds, this could be done easier through the player script
        //but since the player can die multiple ways we need one method
        player.GetComponent<PlayerController>().GetPlayerUI().enabled = false;
        player.GetComponent<PlayerController>().GetPlayerUI().gameObject.SetActive(false);

        //Store score With associated name (max 10 characters)(WIP)



        //Return to main menu (TBA)
    }

    private void WriteToFile()
    {
        Debug.Log("Entering write to file method");
        if (m_nameText.text == "")
        {
            //If the player, for whatever reasons, writes to file with no name, use this placeholder instead
            m_nameText.text = "LONESLDRR";
        }


        StreamWriter scoreWrite = new StreamWriter("scores.txt");
        for (int i = 0; i < scores.Count; i += 2)
        {
            scoreWrite.WriteLine(scores[i].ToUpper().Trim('_') + ":" + scores[i + 1]);
        }


        scoreWrite.WriteLine(m_nameText.text.Trim().ToUpper() + ":" + score.ToString());
        //ALWAYS REMEMBER TO CLOSE
        scoreWrite.Close();

    }
    private void ReadFromFile()
    {
        Debug.Log("Entering read from file method");
        StreamReader scoreRead = new StreamReader("scores.txt");
        string line = "blah";
        string[] tempSplit;
        while ((line = scoreRead.ReadLine()) != null)
        {
            Debug.Log("entering line: " + line);
            //A line is comprised of "name:score"
            tempSplit = line.Split(':');
            //With this, all even numbered spots of the scores list contain names, and their associate score is 1 ahead of that
            scores.Add(tempSplit[0]);
            scores.Add(tempSplit[1]);
        }
        //ALWAYS REMEMBER TO CLOSE
        scoreRead.Close();
    }
}
