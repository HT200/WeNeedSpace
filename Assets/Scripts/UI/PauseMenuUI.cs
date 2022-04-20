using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [SerializeField] private GameObject m_pauseMenuUI;
    [SerializeField] private PlayerUI m_playerUIComponent;

    private GameManager m_gameManager;

    void Start()
    {
        if (m_gameManager == null)
        {
            m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
    }

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
            else if (m_gameManager.m_waveState != WaveState.OVER)
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
        Time.timeScale = 0f;
        m_pauseMenuUI.SetActive(true);
        m_playerUIComponent.enabled = false;
        Cursor.visible = true;
        GameIsPaused = true;
    }

    /// <summary>
    /// Resume the game. This includes deactivating the PauseMenu UI GameObject,
    /// enabling the Player UI Script, restarting the flow of time in game, 
    /// and setting the static boolean variable to false.
    /// </summary>
    public void Resume()
    {
        Time.timeScale = 1f;
        m_pauseMenuUI.SetActive(false);
        m_playerUIComponent.enabled = true;
        Cursor.visible = false;
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
