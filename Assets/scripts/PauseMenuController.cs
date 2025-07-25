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

        // Always re-enable all five buttons when we show the panel
        if (isPaused)
        {
            // Make sure panel itself is enabled first
            pausePanel.SetActive(true);

            // Then explicitly turn on its five child buttons:
            foreach (Transform child in pausePanel.transform)
                child.gameObject.SetActive(true);
        }
        else
        {
            pausePanel.SetActive(false);
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
