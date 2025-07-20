using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EventToastUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI label;
    public float showSeconds = 3f;

    CanvasGroup cg;
    Vector3 startPos, endPos;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        // stash the “hidden” and “shown” local positions
        startPos = transform.localPosition + Vector3.up * 100;
        endPos = transform.localPosition;
        transform.localPosition = startPos;
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
        float t = 0;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = t / 0.3f;
            transform.localPosition = Vector3.Lerp(startPos, endPos, t / 0.3f);
            yield return null;
        }
        cg.alpha = 1;
        transform.localPosition = endPos;

        // wait
        yield return new WaitForSecondsRealtime(showSeconds);

        // fade out
        t = 0;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = 1 - t / 0.3f;
            yield return null;
        }
        cg.alpha = 0;
        transform.localPosition = startPos;
    }
}
