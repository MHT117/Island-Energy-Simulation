using UnityEngine;
using TMPro;    // ← make sure this is TMPro, not TMPro

public class LedgerPanel : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI supplyTxt;
    public TextMeshProUGUI demandTxt;
    public TextMeshProUGUI surplusTxt;
    public TextMeshProUGUI clockTxt;

    [Header("Resilience UI (temp)")]
    public TextMeshProUGUI secTxt;
    public TextMeshProUGUI eqTxt;
    public TextMeshProUGUI susTxt;
    public TextMeshProUGUI adaTxt;

    void Update()
    {
        // — existing power numbers —
        supplyTxt.text = $"Supply  {PowerManager.I.supplyMW:F1} MW";
        demandTxt.text = $"Demand  {PowerManager.I.demandMW:F3} MW";
        surplusTxt.text = $"Surplus {PowerManager.I.surplusMW:F1} MW";

        // — clock —
        int m = TimeSystem.I.MinuteOfDay;
        int hr = m / 60;
        int mn = m % 60;
        clockTxt.text = $"Day {TimeSystem.I.Day}  {hr:00}:{mn:00}";

        // — resilience read‐out —
        secTxt.text = $"SEC  {ResilienceManager.I.R_Sec:F1}";
        eqTxt.text = $"EQ   {ResilienceManager.I.R_Eq:F1}";
        susTxt.text = $"SUS  {ResilienceManager.I.R_Sus:F1}";
        adaTxt.text = $"ADA  {ResilienceManager.I.R_Ada:F1}";
    }
}
