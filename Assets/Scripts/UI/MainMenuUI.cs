using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    [SerializeField] private GameObject m_mainMenuUI;
    [SerializeField] private GameObject m_leaderboardUI;
    [SerializeField] private GameObject m_howToPlayUI;
    [SerializeField] private GameObject m_creditsUI;

    // Update is called once per frame
    void Update()
    {
        m_camera.transform.Rotate(0f, 2 * Time.deltaTime, 0f);
    }

    public void BackButton()
    {
        m_mainMenuUI.SetActive(true);
        m_leaderboardUI.SetActive(false);
        m_howToPlayUI.SetActive(false);
        m_creditsUI.SetActive(false);
    }

    public void PlayButton()
    {
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void LeaderboardButton()
    {
        m_mainMenuUI.SetActive(false);
        m_leaderboardUI.SetActive(true);
    }

    public List<string> GetTop10Scores()
    {
        Debug.Log("Gathering Top 10 Scores for Leaderboard display");

        List<string> top10 = new List<string>();
        StreamReader scoreReader = new StreamReader("scores.txt");

        int counter = 10;
        string line = "";
        string[] tempSplit;
        while ((line = scoreReader.ReadLine()) != null || counter > 0)
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

    public void HowToPlayButton()
    {
        m_mainMenuUI.SetActive(false);
        m_howToPlayUI.SetActive(true);
    }

    public void CreditsButton()
    {
        m_mainMenuUI.SetActive(false);
        m_creditsUI.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
