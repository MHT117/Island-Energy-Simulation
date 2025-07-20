// GameManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("Economy")]
    public int startingMoney = 3000;
    public TextMeshProUGUI moneyText;

    [Header("Notifications")]
    public TextMeshProUGUI notificationText;  // drag your NotificationText here

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
            moneyText.text = $"${_money:N0}";
        }
    }

    void Awake()
    {
        Time.timeScale = 1f;
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        Money = startingMoney;
    }

    void Start()
    {
        TimeSystem.I.OnNewDay += OnNewDay;
    }

    void OnEnable()
    {
        if (endGamePanel == null) return;
        var buttons = endGamePanel.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            if (btn.name.Contains("Restart"))
                btn.onClick.AddListener(() => SceneManager.LoadScene(0));
            else if (btn.name.Contains("Quit"))
            {
#if UNITY_WEBGL
                btn.onClick.AddListener(() => Application.OpenURL("about:blank"));
#else
                btn.onClick.AddListener(Application.Quit);
#endif
            }
        }
    }

    private void OnNewDay(int dayNumber)
    {
        // → 1) compute weather
        var w = WeatherSystem.I.Current;
        string weatherMsg = $"Day {dayNumber}: Sun {w.sun:F2}  Wind {w.wind:F2}";

        // → 2) compute and spend maintenance
        int bill = 0;
        foreach (var src in Object.FindObjectsByType<EnergySourceInstance>(
    FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            bill += src.data.maintenancePerDay;
        }

        foreach (var con in Object.FindObjectsByType<ConsumerInstance>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            bill += con.data.maintenancePerDay;
        }

        Spend(bill);
        string billMsg = $"Maintenance: ${bill}";

        // → 3) show both lines in one notification
        ShowNotification($"{weatherMsg}\n{billMsg}");

        if (Money <= 0)
            HandleBudgetFail();
    }

    public void ShowNotification(string msg)
    {
        if (notificationText != null)
            notificationText.text = msg;
    }

    public bool CanAfford(int amount) => Money >= amount;
    public void Spend(int amount) => Money -= amount;

    void HandleBudgetFail()
    {
        Debug.LogError("BANKRUPT! Money ≤ 0.");
        if (pauseOnBudgetFail) Time.timeScale = 0f;
        if (budgetFailPanel != null) budgetFailPanel.SetActive(true);
    }

    public void InstanceBlackout() => TriggerEndGame(false);

    public void TriggerEndGame(bool win)
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0f;
        endGamePanel.SetActive(true);
        endGameLabel.text = win ? "YOU WIN!" : "GAME OVER";
    }
}
