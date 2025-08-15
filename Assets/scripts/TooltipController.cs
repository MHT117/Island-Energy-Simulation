using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class TooltipController : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ScriptableObject data;     // drag SO here

    // static refs: one shared panel + label
    static GameObject panel;
    static TextMeshProUGUI label;

    Coroutine hideCo;

    void Awake()
    {
        if (panel == null)
        {
            // 1) find the Canvas in the scene
            var canvasGO = GameObject.Find("Canvas");
            if (canvasGO == null)
            {
                Debug.LogError("TooltipController: No GameObject named 'Canvas' found!");
                return;
            }

            // 2) look for the child named TooltipPanel (works even if inactive)
            var tipTrans = canvasGO.transform.Find("TooltipPanel");
            if (tipTrans == null)
            {
                Debug.LogError("TooltipController: 'Canvas' exists but has no child 'TooltipPanel'!");
                return;
            }

            panel = tipTrans.gameObject;

            // 3) grab the TextMeshPro label inside it
            label = panel.GetComponentInChildren<TextMeshProUGUI>();
            if (label == null)
                Debug.LogError("TooltipController: TooltipPanel has no TMP label child!");

            // 4) ensure it starts hidden
            panel.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (!data || panel == null) return;

        // Base title (pretty name)
        string nameOnly = data.name.Replace("_SO", "").Replace("_", " ");

        // Try to enrich if this is an EnergySourceSO
        string extra = BuildExtraLine(data);

        label.text = string.IsNullOrEmpty(extra) ? nameOnly : $"{nameOnly}\n<size=80%>{extra}</size>";
        panel.SetActive(true);

        if (hideCo != null) StopCoroutine(hideCo);
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (hideCo != null) StopCoroutine(hideCo);
        hideCo = StartCoroutine(HideDelay());
    }

    IEnumerator HideDelay()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        panel.SetActive(false);
    }

    // --- helpers for the tooltip to show the values ---
    string BuildExtraLine(ScriptableObject so)
    {
        var t = so.GetType();
        if (t.Name != "EnergySourceSO") return "";

        int cost = GetInt(so, "buildCost", -1);
        float mw = GetFloat(so, "baseOutputMW", -1f);
        int maint = GetInt(so, "maintenanceDay", -1);     // if daily maintenance is kept

        string s = "";
        if (cost >= 0) s += $"Cost ${cost}";
        if (mw >= 0) s += (s == "" ? "" : "  |  ") + $"Output {mw:0.##} MW";
        if (maint >= 0) s += $"  |  Maintanance ${maint}/day";
        return s;
    }

    int GetInt(object o, string field, int def)
    {
        var f = o.GetType().GetField(field);
        return f != null && f.FieldType == typeof(int) ? (int)f.GetValue(o) : def;
    }
    float GetFloat(object o, string field, float def)
    {
        var f = o.GetType().GetField(field);
        return f != null && f.FieldType == typeof(float) ? (float)f.GetValue(o) : def;
    }
}
