using UnityEngine;
using TMPro;

public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] TMP_InputField moneyField;
    [SerializeField] TMP_InputField dayLenField;

    void OnEnable()
    {
        // Populate the two fields when the panel opens
        moneyField.text = GameManager.I.startingMoney.ToString();
        dayLenField.text = TimeSystem.I.realSecondsPerGameMinute.ToString("0.##");
    }

    public void BtnApply()
    {
        // Save starting money
        if (int.TryParse(moneyField.text, out int money))
            GameManager.I.startingMoney = money;

        // Save seconds per game-minute (clamp to a sensible minimum)
        if (float.TryParse(dayLenField.text, out float sec))
            TimeSystem.I.realSecondsPerGameMinute = Mathf.Max(0.05f, sec);

        gameObject.SetActive(false);
    }

    public void BtnCancel()
    {
        // Simply close the panel
        gameObject.SetActive(false);
    }
}
