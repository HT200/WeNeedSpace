using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private GameObject m_nameColumn;
    [SerializeField] private GameObject m_scoreColumn;
    [SerializeField] private GameObject m_nameTextPrefab;
    [SerializeField] private GameObject m_scoreTextPrefab;

    // Start is called before the first frame update
    void Start()
    {
        List<string> top10Scores = GetTop10Scores();
        
        for (int i = 0; i < (top10Scores.Count / 2); i++)
        {
            int index = 2 * i;
            GameObject name = Instantiate(m_nameTextPrefab, m_nameColumn.transform);
            name.GetComponent<Text>().text = top10Scores[index];
            GameObject score = Instantiate(m_scoreTextPrefab, m_scoreColumn.transform);
            score.GetComponent<Text>().text = top10Scores[index + 1];
        }
    }

    /// <summary>
    /// Get the Top 10 scores from the "scores.txt" file. If there are less than 10 
    /// scores available, all scores will be returned.
    /// </summary>
    /// <returns>A list of Names at even indices and associated Scores at odd indices</returns>
    public List<string> GetTop10Scores()
    {
        Debug.Log("Gathering Top 10 Scores for Leaderboard display");

        StreamReader scoreReader;
        //If you have scores of your own, display those 
        if(System.IO.File.Exists(Application.persistentDataPath + "scores.txt"))
        {
            scoreReader = new StreamReader(Application.persistentDataPath + "scores.txt");
        }
        else
        {
            //If this is the first run through, use the default one instead
            scoreReader = new StreamReader(Application.streamingAssetsPath + "/Text/scores.txt");
        }
        List<string> top10 = new List<string>();
        int counter = 10;
        string line = "";
        string[] tempSplit;
        while ((line = scoreReader.ReadLine()) != null && counter > 0)
        {
            Debug.Log("Read line: " + line);
            // A line is comprised of "name:score"
            tempSplit = line.Split(':');
            // With this, all even numbered spots of the scores list contain names, and their associate score is 1 ahead of that
            top10.Add(tempSplit[0].Trim('_').ToUpper());
            top10.Add(tempSplit[1]);
        }

        // ALWAYS REMEMBER TO CLOSE
        scoreReader.Close();

        return top10;
    }
}
