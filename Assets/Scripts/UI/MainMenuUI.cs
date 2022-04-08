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
