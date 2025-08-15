using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EventToastUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI label;
    public float showSeconds = 5f;

    CanvasGroup cg;
    RectTransform rt;
    Vector2 startPos, endPos;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        rt = (RectTransform)transform;

        // slide from slightly above the final anchored position
        endPos = rt.anchoredPosition;
        startPos = endPos + new Vector2(0f, 100f);
        rt.anchoredPosition = startPos;
        cg.alpha = 0f;
    }

    public void Show(ClimateEventSO ev)
    {
        icon.sprite = ev.icon;
        label.text = ev.displayName;
        StopAllCoroutines();
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        // fade + slide in (0.3s)
        float t = 0f;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            float p = t / 0.3f;
            cg.alpha = p;
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, p);
            yield return null;
        }
        cg.alpha = 1f; rt.anchoredPosition = endPos;

        yield return new WaitForSecondsRealtime(showSeconds);

        // fade out (0.3s)
        t = 0f;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            float p = t / 0.3f;
            cg.alpha = 1f - p;
            yield return null;
        }
        cg.alpha = 0f;
        rt.anchoredPosition = startPos;
    }
}
