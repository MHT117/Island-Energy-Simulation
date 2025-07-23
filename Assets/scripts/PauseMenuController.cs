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
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        pausePanel.SetActive(isPaused);
        if (!isPaused)
        {
            settingsPanel.SetActive(false);
            savePanel.SetActive(false);
            loadPanel.SetActive(false);
        }
    }

    // Public hooks for buttons
    public void BtnResume() => TogglePause();
    public void BtnSettings() { settingsPanel.SetActive(true); }
    public void BtnSave() { savePanel.SetActive(true); }
    public void BtnLoad() { loadPanel.SetActive(true); }
    public void BtnQuit() => SceneManager.LoadScene("MainMenu");

}
