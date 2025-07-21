using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class TooltipController : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] ScriptableObject data;     // drag your SO here

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
        if (data == null) return;

        // remove suffix/prefix to get nice name
        string nameOnly = data.name
            .Replace("_SO", "")
            .Replace("_", " ");
        label.text = nameOnly;
        panel.SetActive(true);

        // cancel any pending hide
        if (hideCo != null) StopCoroutine(hideCo);
    }

    public void OnPointerExit(PointerEventData e)
    {
        // optional delay before hiding
        if (hideCo != null) StopCoroutine(hideCo);
        hideCo = StartCoroutine(HideDelay());
    }

    IEnumerator HideDelay()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        panel.SetActive(false);
    }
}
