using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject m_mainmenuUI;
    [SerializeField] private GameObject m_leaderboardUI;
    [SerializeField] private GameObject m_howtoplayUI;
    [SerializeField] private GameObject m_creditsUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        SceneManager.LoadScene("MVPScene", LoadSceneMode.Single);
    }

    public void Leaderboard()
    {
        m_leaderboardUI.SetActive(true);
    }

    public void HowToPlay()
    {
        m_howtoplayUI.SetActive(true);
    }

    public void Credits()
    {
        m_mainmenuUI.SetActive(false);
        m_creditsUI.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
