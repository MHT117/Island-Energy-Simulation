using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StakeholderBarUI : MonoBehaviour
{
    [Header("UI References")]
    public Image fill;                 // Drag the “Fill” Image here
    public TextMeshProUGUI label;      // Drag the “Label” TMP text here

    [HideInInspector]
    public StakeholderSO data;         // Assigned at runtime

    void Awake()
    {
        // Make sure our bar is set up as a horizontal filled image
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
    }

    /// <summary>
    /// Refreshes the bar to match data.satisfaction (0–100).
    /// </summary>
    public void Refresh()
    {
        float pct = data.satisfaction / 100f;
        fill.fillAmount = pct;

        // Color‐code: green ≥80%, yellow ≥50%, red otherwise
        fill.color = pct >= 0.8f ? Color.green
                   : pct >= 0.5f ? Color.yellow
                   : Color.red;

        label.text = $"{data.displayName}  {data.satisfaction:0}%";
    }
}

