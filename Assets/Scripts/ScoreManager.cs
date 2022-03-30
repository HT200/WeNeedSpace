using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    // TODO: Add more...

    // UI Elements
    [SerializeField] private Text m_scoreText;

    //for storing score/name pairs in I/O
    private Vector3 ogSpaceIndicator;
    private Vector3 endSpaceIndicator;

    [SerializeField] private Text m_nameText;
    [SerializeField] private Text m_spaceIndicator;

    //For altering the name
    int nameIndex;
    const string alphabet = "_ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()+=;\"~`<>?";

    List<string> names;
    List<int> scores;

    List<string> HighScores;



    // Start is called before the first frame update
    void Start()
    {
        names = new List<string>();
        scores = new List<int>();
        HighScores = new List<string>();

        ogSpaceIndicator = m_spaceIndicator.gameObject.transform.position;
        endSpaceIndicator = new Vector3(ogSpaceIndicator.x + 250, ogSpaceIndicator.y, ogSpaceIndicator.z);

        m_currentScore = 0;
        m_totalKills = 0;

        nameIndex = 0;
        m_nameText.text = "__________";
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
        Debug.Log("Updating Score from " + m_currentScore + " to " + (m_currentScore + updateAmount));
        // Add to the player's score
        m_currentScore += updateAmount;
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

    public void GetName()
    {
        if (!m_nameText.gameObject.activeInHierarchy)
        {
            m_nameText.gameObject.SetActive(true);
        }

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
                    m_spaceIndicator.gameObject.transform.position.x + 27,
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
                m_spaceIndicator.gameObject.transform.position.x - 27,
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

    public void DisplayNameText()
    {
        m_nameText.gameObject.SetActive(true);
        m_spaceIndicator.gameObject.SetActive(true);
    }

    public void WriteToFile()
    {
        SortLists();
        Debug.Log("Entering write to file method");
        if (m_nameText.text.Trim('_') == "")
        {
            //If the player, for whatever reasons, writes to file with no name, use this placeholder instead
            m_nameText.text = "LONESLDRR";
        }

        StreamWriter scoreWrite = new StreamWriter("scores.txt");
        for (int i = 0; i < HighScores.Count; i += 2)
        {
            scoreWrite.WriteLine(HighScores[i].Trim('_').ToUpper() + ":" + HighScores[i + 1]);
        }

        scoreWrite.WriteLine(m_nameText.text.Trim('_').ToUpper() + ":" + m_currentScore.ToString());
        //ALWAYS REMEMBER TO CLOSE
        scoreWrite.Close();
    }

    public void ReadFromFile()
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
            names.Add(tempSplit[0].Trim('_').ToUpper());
            scores.Add(int.Parse(tempSplit[1]));
        }

        names.Add(m_nameText.text.Trim('_').ToUpper());
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
            scores[index] = 0;
        }
    }
}
