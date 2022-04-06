using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] private GameObject m_pauseMenuUI;
    [SerializeField] private PlayerUI m_playerUIComponent;
    
    // Update is called once per frame
    void Update()
    {
        // Pause the game on Escape or P key press
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// Pause the game. This includes activating the PauseMenu UI GameObject,
    /// disabling the Player UI Script, pausing the flow of time in game, 
    /// and setting the static boolean variable to true.
    /// </summary>
    void Pause()
    {
        m_pauseMenuUI.SetActive(true);
        m_playerUIComponent.enabled = false;
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    /// <summary>
    /// Resume the game. This includes deactivating the PauseMenu UI GameObject,
    /// enabling the Player UI Script, restarting the flow of time in game, 
    /// and setting the static boolean variable to false.
    /// </summary>
    public void Resume()
    {
        m_pauseMenuUI.SetActive(false);
        m_playerUIComponent.enabled = true;
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    /// <summary>
    /// Quit from the game and return to the Main Menu.
    /// </summary>
    public void Quit()
    {
        // Temporarily resume the game before returning to the Main Menu 
        // so everything is reset, mainly time scale
        Resume();
        SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
    }
}
