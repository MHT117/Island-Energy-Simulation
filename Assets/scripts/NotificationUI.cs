using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class NotificationUI : MonoBehaviour
{
    public static NotificationUI I { get; private set; }
    public TextMeshProUGUI textField;
    public float fadeTime = 0.3f;
    public float holdTime = 2f;

    CanvasGroup cg;
    Coroutine current;

    void Awake()
    {
        if (I == null) I = this;
        else Destroy(gameObject);

        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;
    }

    public void Show(string msg)
    {
        textField.text = msg;
        if (current != null) StopCoroutine(current);
        current = StartCoroutine(DoShow());
    }

    IEnumerator DoShow()
    {
        // fade in
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = t / fadeTime;
            yield return null;
        }
        cg.alpha = 1f;

        // hold
        yield return new WaitForSecondsRealtime(holdTime);

        // fade out
        t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = 1f - (t / fadeTime);
            yield return null;
        }
        cg.alpha = 0f;
    }
}
