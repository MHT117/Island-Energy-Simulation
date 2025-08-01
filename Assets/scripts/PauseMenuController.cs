using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject savePanel;
    [SerializeField] GameObject loadPanel;

    bool isPaused;

    void Update()
    {
        // if the tutorial is running, no one can pause yet
        if (GameManager.I.tutorialActive)
            return;
        // if a prompt or tutorial is blocking UI, bail out
        if (GameManager.I.tutorialActive) return;


        // otherwise, pressing Escape still toggles the pause menu as before
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }


    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        // Activate root panel only
        pausePanel.SetActive(isPaused);

        // Deactivate sub-panels every time Pause toggles
        settingsPanel.SetActive(false);
        savePanel.SetActive(false);
        loadPanel.SetActive(false);

        Debug.Log(isPaused ? "[PauseMenu] Open" : "[PauseMenu] Close");
    }

    // Buttons invoke these public methods
    public void BtnResume()
    {
        Debug.Log("[PauseMenu] Resume");
        TogglePause();
    }

    public void BtnSettings()
    {
        Debug.Log("[PauseMenu] Open Settings");
        settingsPanel.SetActive(true);
    }

    public void BtnSave()
    {
        Debug.Log("[PauseMenu] Open Save");
        savePanel.SetActive(true);
    }

    public void BtnLoad()
    {
        Debug.Log("[PauseMenu] Open Load");
        loadPanel.SetActive(true);
    }

    public void BtnQuit()
    {
        Debug.Log("[PauseMenu] Quit");
        SceneManager.LoadScene("MainMenu");
    }
}
