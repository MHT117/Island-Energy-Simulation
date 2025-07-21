using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("Economy")]
    public int startingMoney = 1000;
    public TextMeshProUGUI moneyText;

    [Header("Lose Condition")]
    public bool pauseOnBudgetFail = true;
    public GameObject budgetFailPanel;

    [Header("End-game UI")]
    public GameObject endGamePanel;
    public TextMeshProUGUI endGameLabel;

    bool gameEnded = false;

    int _money;
    public int Money
    {
        get => _money;
        private set
        {
            _money = value;
            if (moneyText != null)
                moneyText.text = $"${_money:N0}";
        }
    }
    // this is the save game trigger
    void Update()
    {
        // existing code…

        // ─── Quick‐save key ───────────────────────────────────────────
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveSystem.SaveGame();
            Debug.Log("Game manually saved.");
        }
    }
    // save game on quite
    void OnApplicationQuit()
    {
        // Auto‐save when the player closes or reloads the game
        SaveSystem.SaveGame();
        Debug.Log("Game auto‐saved on quit.");
    }

    void Awake()
    {
        // cap at 60 FPS
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        // ensure we start unpaused
        Time.timeScale = 1f;  // always start unpaused in Play
        if (I != null)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
        Money = startingMoney;
    }

    void OnEnable()
    {
        // wire up Restart/Quit buttons under your EndGamePanel
        if (endGamePanel == null) return;
        foreach (var btn in endGamePanel.GetComponentsInChildren<Button>())
        {
            if (btn.name.Contains("Restart"))
                btn.onClick.AddListener(() => SceneManager.LoadScene(0));
            else if (btn.name.Contains("Quit"))
                btn.onClick.AddListener(Application.Quit);
        }
    }

    void Start()
    {
        // charge maintenance each new day (if you have that hooked up)
        TimeSystem.I.OnNewDay += ChargeDailyMaintenance;
    }

    public bool CanAfford(int amount) => Money >= amount;
    public void Spend(int amount) => Money -= amount;
    // ← new! so SaveSystem.LoadGame() can restore your cash
    public void SetMoney(int amount) => Money = amount;

    /// <summary>
    /// Called each new day; deducts all building maintenance.
    /// </summary>
    void ChargeDailyMaintenance(int dayNumber)
    {
        int bill = 0;
        // (your existing code to sum maintenance goes here)
        Spend(bill);
        Debug.Log($"Day {dayNumber}: maintenance bill ${bill}");
        if (Money <= 0)
            TriggerEndGame(false);
    }

    /// <summary>
    /// Call this to end the game (true=win, false=lose).
    /// </summary>
    public void TriggerEndGame(bool win)
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0f;
        if (endGamePanel) endGamePanel.SetActive(true);
        if (endGameLabel) endGameLabel.text = win ? "YOU WIN!" : "GAME OVER";
    }

    // If you still have a separate Blackout handler, make it call TriggerEndGame(false):
    public void InstanceBlackout()
    {
        Debug.LogWarning("BLACKOUT – insufficient power!");
        TriggerEndGame(false);
    }

    // If you had a separate stakeholder‐lose handler, have it use TriggerEndGame(false) too
    public void HandleStakeholderLose(StakeholderSO st)
    {
        Debug.LogWarning($"{st.displayName} is unhappy (<{st.loseBelow}%). Game over.");
        TriggerEndGame(false);
    }
}
