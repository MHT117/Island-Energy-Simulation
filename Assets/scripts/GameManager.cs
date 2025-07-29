using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }
    [HideInInspector] public bool tutorialActive = false;

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

    void Awake()
    {
        // cap at 60 FPS
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        // ensure we start unpaused
        Time.timeScale = 1f;

        // singleton setup
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);

        // initialize money
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

    public static bool LoadLatest()
    {
        for (int i = 1; i <= 3; i++)
            if (SaveSystem.LoadGame(i))
                return true;
        return false;
    }
    void Start()
    { // attempt to load an existing save; returns true if some slot loaded
        bool didLoad = SaveSystem.LoadLatest();
        if (!didLoad)
        {
            Debug.Log("[Tutorial] spawning TutorialCanvas prefab");
            tutorialActive = true;
            Time.timeScale = 0f;
            Instantiate(Resources.Load<GameObject>("TutorialCanvas"));
            return;
        }

        // charge maintenance each new day
        TimeSystem.I.OnNewDay += ChargeDailyMaintenance;
    }

    void Update()
    {
        // quick‐save key
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveSystem.SaveGame();
            Debug.Log("Game manually saved.");
        }
    }

    void OnApplicationQuit()
    {
        // auto‐save on quit
        SaveSystem.SaveGame();
        Debug.Log("Game auto‐saved on quit.");
    }

    public bool CanAfford(int amount) => Money >= amount;
    public void Spend(int amount) => Money -= amount;
    public void SetMoney(int amount) => Money = amount;

    /// <summary>
    /// Called each new game day to sum and deduct maintenance costs.
    /// </summary>
    void ChargeDailyMaintenance(int dayNumber)
    {
        // ---- calculate maintenance ----
        int bill = 0;
        foreach (var src in Object.FindObjectsByType<EnergySourceInstance>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            bill += src.data.maintenancePerDay;
        foreach (var con in Object.FindObjectsByType<ConsumerInstance>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            bill += con.data.maintenancePerDay;

        // ---- deduct cash ----
        Spend(bill);

        // ---- pull in the weather snapshot ----
        var w = WeatherSystem.I.Current;

        // ---- build one combined message ----
        string toast = $"Day {dayNumber}:  Sun {w.sun:F2}  Wind {w.wind:F2}\n" +
                       $"Maintenance bill ${bill}";

        Debug.Log(toast);

        // ---- show on-screen ----
        if (NotificationUI.I != null)
            NotificationUI.I.Show(toast);

        // ---- bankruptcy check ----
        if (Money <= 0)
            TriggerEndGame(false);
    }

    /// <summary>
    /// Ends the game: pauses and shows the end‐game panel.
    /// </summary>
    public void TriggerEndGame(bool win)
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f;

        if (endGamePanel) endGamePanel.SetActive(true);
        if (endGameLabel) endGameLabel.text = win ? "YOU WIN!" : "GAME OVER";
    }

    public void InstanceBlackout()
    {
        Debug.LogWarning("BLACKOUT – insufficient power!");
        TriggerEndGame(false);
    }

    public void HandleStakeholderLose(StakeholderSO st)
    {
        Debug.LogWarning($"{st.displayName} is unhappy (<{st.loseBelow}%). Game over.");
        TriggerEndGame(false);
    }
}
