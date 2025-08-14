using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SettingsPanelController : MonoBehaviour
{
    [SerializeField] TMP_InputField moneyField;
    [SerializeField] TMP_InputField dayLenField;
    [SerializeField] Slider musicSlider;


    void OnEnable()
    {
        // Populate the two fields when the panel opens
        moneyField.text = GameManager.I.startingMoney.ToString();
        dayLenField.text = TimeSystem.I.realSecondsPerGameMinute.ToString("0.##");
        if (AudioManager.I && musicSlider)
            musicSlider.value = AudioManager.I.Volume;
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

    public void OnMusicSliderChanged(float v)
    {
        if (AudioManager.I) AudioManager.I.SetMusicVolume(v);
    }

}
