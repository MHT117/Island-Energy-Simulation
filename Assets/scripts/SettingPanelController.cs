using System.Linq;
using TMPro;
using UnityEngine;

public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] TMP_InputField moneyField;
    [SerializeField] TMP_InputField windField;
    [SerializeField] TMP_InputField dayLenField;

    EnergySourceSO windSO;

    void OnEnable()
    {
        // Starting money (directly on GameManager)
        moneyField.text = GameManager.I.Money.ToString();

        // Find your Wind SO asset (by displayName), then show its placement limit
        windSO = Resources
            .LoadAll<EnergySourceSO>("Data/EnergySources")
            .FirstOrDefault(so => so.displayName.Contains("Wind"));
        windField.text = windSO != null
            ? windSO.maxPlantsAllowed.ToString()
            : "0";

        // Seconds per game-minute
        dayLenField.text = TimeSystem.I.realSecondsPerGameMinute.ToString("0.##");
    }

    public void BtnApply()
    {
        // Money
        if (int.TryParse(moneyField.text, out int m))
            GameManager.I.SetMoney(m);

        // Wind max
        if (windSO != null && int.TryParse(windField.text, out int w))
            windSO.maxPlantsAllowed = w;

        // Day length
        if (float.TryParse(dayLenField.text, out float s))
            TimeSystem.I.realSecondsPerGameMinute = Mathf.Max(0.05f, s);

        gameObject.SetActive(false);
    }

    public void BtnCancel() => gameObject.SetActive(false);
}
