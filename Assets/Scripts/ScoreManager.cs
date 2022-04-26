using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    // Score variables
    private int m_currentScore;
    private int m_totalKills;

    // Score Types
    private int m_scoreEnemyHit = 5;
    private int m_scoreWaveClear = 100;
    // TODO: Add more...

    // Combo for accurate shots
    private int m_scoreCombo = 20;
    public int m_scoreMultiplier = 1;
    // TODO: Add more...

    // UI Elements
    [SerializeField] private Text m_scoreText;
    [SerializeField] private Text m_warningText;

    //for storing score/name pairs in I/O
    private Vector3 ogSpaceIndicator;
    private Vector3 endSpaceIndicator;

    //For altering the name
    private int m_nameIndex;
    [SerializeField] private GameObject m_nameTextParent;
    private List<Text> m_nameTextCharList;
    private const string m_alphabet = "_ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()+=;\"~`<>?";

    List<string> names;
    List<int> scores;
    List<string> HighScores;
    private List<string> slurs;
    private string currPlayerName;

    // Start is called before the first frame update
    void Start()
    {
        names = new List<string>();
        scores = new List<int>();
        HighScores = new List<string>();

        m_currentScore = 0;
        m_totalKills = 0;
        m_scoreMultiplier = 1;

        m_nameIndex = 0;
        m_nameTextCharList = new List<Text>();
        
        GetSlurList();
        
        if (m_nameTextParent == null) return;
        
        // Add each child Text component to the internal List
        foreach(Transform child in m_nameTextParent.transform)
        {
            m_nameTextCharList.Add(child.GetComponent<Text>());
        }
    }

    void Update()
    {
        m_nameTextCharList[m_nameIndex].color = Color.red;
    }

    /// <summary>
    /// Update the player's score when they clear a wave.
    /// </summary>
    public void UpdateScoreWaveCleared()
    {
        UpdateScore(m_scoreWaveClear);
    }

    /// <summary>
    /// Update the player's score when they hit an enemy.
    /// </summary>
    public void UpdateScoreEnemyHit()
    {
        // Apply combo multiplier, then update the score
        int updateAmount = m_scoreEnemyHit + m_scoreCombo;
        UpdateScore(updateAmount);
    }

    /// <summary>
    /// Update score main function. This logs the score update, updates the current score, 
    /// and shows the updated score in the UI.
    /// </summary>
    /// <param name="updateAmount"></param>
    private void UpdateScore(int updateAmount)
    {
        // Log the Score update
        Debug.Log("Updating Score from " + m_currentScore + " to " + (m_currentScore + updateAmount * m_scoreMultiplier));
        // Add to the player's score
        m_currentScore += updateAmount * m_scoreMultiplier;
        // Update the UI
        m_scoreText.text = "Score: " + m_currentScore;
    }

    /// <summary>
    /// Applies a bonus based on how much time it took for the player to destroy the wave
    /// </summary>
    public void TimeBonus(float waveTime)
    {
        //If the player defeated the wave in under a minute, flat 300 bonus
        if (waveTime < 60.0f)
        {
            m_currentScore += 300;
        }
        //If between 1-3 minutes, use variable, its should be 300 at 60, 0 at 180
        else if (waveTime < 180.0f)
        {
            m_currentScore += (int)(300 - 2.5 * (waveTime - 60.0f));
        }
        //Otherwise give 0 points
    }

    /// <summary>
    /// Get the player's current combo multiplier
    /// </summary>
    /// <returns>The player's current combo multiplier</returns>
    public int GetCombo()
    {
        return m_scoreCombo;
    }

    /// <summary>
    /// Set the player's current combo multiplier
    /// </summary>
    public void SetCombo(int newCombo)
    {
        m_scoreCombo = newCombo;
    }

    public void IncrementKill()
    {
        m_totalKills++;
        m_scoreCombo += 12;
    }

    public void DecrementCombo()
    {
        if (m_scoreCombo > 1)
        {
            m_scoreCombo -= 2;
        }
    }

    /// <summary>
    /// Note: m_nameIndex ranges from 0-9
    /// </summary>
    public void GetName()
    {
        if (!m_nameTextParent.gameObject.activeInHierarchy)
        {
            m_nameTextParent.gameObject.SetActive(true);
            m_warningText.gameObject.SetActive(false);
        }

        // Get the current character and make it red
        Text currentChar = m_nameTextCharList[m_nameIndex];
        currentChar.color = Color.red;

        // Update the current index
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Make the current character white before updating the index
            currentChar.color = Color.white;
            m_nameIndex = (m_nameIndex + 1) % 10;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Make the current character white before updating the index
            currentChar.color = Color.white;

            // For whatever reason, modulus doesn't work with negative numbers (even though it should?).
            // I've replicated the effect with this. Need to do the same for the down arrow logic.
            if (m_nameIndex == 0)
            {
                m_nameIndex = 9;
            }
            else
            {
                m_nameIndex--;
            }
        }

        // Update the current character
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_warningText.gameObject.SetActive(false);
            currentChar.text = m_alphabet[(m_alphabet.IndexOf(currentChar.text) + 1) % m_alphabet.Length].ToString();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_warningText.gameObject.SetActive(false);
            if (m_alphabet.IndexOf(currentChar.text) == 0)
            {
                currentChar.text = m_alphabet[m_alphabet.Length - 1].ToString();
            }
            else
            {
                currentChar.text = m_alphabet[(m_alphabet.IndexOf(currentChar.text) - 1) % m_alphabet.Length].ToString();
            }
        }

        // Submit the current player's name
        if (Input.GetKeyDown(KeyCode.Return))
        {
            foreach (Text child in m_nameTextCharList) currPlayerName += child.text;

            if (IsContainSlur())
            {
                m_warningText.gameObject.SetActive(true);                
                return;
            }

            // This block needs a scene or state transition to avoid a player writing to file multiple times
            ReadFromFile();
            WriteToFile();
            SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
        }
    }

    public void WriteToFile()
    {
        SortLists();
        //Always write to the persistent data path, even if the file isn't there yet, it'll create it
        StreamWriter scoreWrite = new StreamWriter(Application.persistentDataPath + "scores.txt");
        for (int i = 0; i < HighScores.Count; i += 2)
        {
            scoreWrite.WriteLine(HighScores[i].Trim('_').ToUpper() + ":" + HighScores[i + 1]);
        }

        //ALWAYS REMEMBER TO CLOSE
        scoreWrite.Close();
    }

    public void ReadFromFile()
    {
        names.Clear();
        scores.Clear();
        HighScores.Clear();
        //Forward declare score reader to avoid warnings/errors
        StreamReader scoreRead;
        //If this is the first time running, the score file wont exist
        if (System.IO.File.Exists(Application.persistentDataPath + "scores.txt"))
        {
            //if it does exist, use it
            scoreRead = new StreamReader(Application.persistentDataPath + "scores.txt");
        }
        else
        {
            //if it doesn't, this is the first time so use the default one
            scoreRead = new StreamReader(Application.streamingAssetsPath + "/Text/scores.txt");
        }
        string line = "blah";
        string[] tempSplit;
        while ((line = scoreRead.ReadLine()) != null)
        {
            Debug.Log("entering line: " + line);
            //A line is comprised of "name:score"
            tempSplit = line.Split(':');
            //With this, all even numbered spots of the scores list contain names, and their associate score is 1 ahead of that
            names.Add(tempSplit[0].Trim('_').ToUpper());
            scores.Add(int.Parse(tempSplit[1]));
        }
        
        names.Add(currPlayerName);
        scores.Add(m_currentScore);

        //ALWAYS REMEMBER TO CLOSE
        scoreRead.Close();
    }

    public void SortLists()
    {
        int max = -1;
        int index = 0;
        for (int p = 0; p < scores.Count; p++)
        {
            max = -1;
            index = 0;
            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i] > max)
                {
                    index = i;
                    max = scores[i];
                }
            }
            HighScores.Add(names[index]);
            HighScores.Add(scores[index].ToString());
            scores[index] = -1;
        }
    }

    public void GetSlurList()
    {
        //Since the player cannot create slurs, you can always take this from the streamed assets
        slurs = new List<string>();
        StreamReader slurReader = new StreamReader(Application.streamingAssetsPath + "/Text/slurs.txt");
        
        string line = "";
        while ((line = slurReader.ReadLine()) != null) slurs.Add(line);
        
        slurReader.Close();
    }

    public bool IsContainSlur()
    {
        foreach (var slur in slurs)
        {
            Debug.Log(slur);
        }
        return slurs.Any(slur => currPlayerName.ToLower().Contains(slur));
    }
}
