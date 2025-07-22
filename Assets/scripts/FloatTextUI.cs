using UnityEngine;
using TMPro;
using System.Collections;

public class FloatTextUI : MonoBehaviour
{
    public float lifeSeconds = 5f;      // optional tweak: shorter default life
    CanvasGroup cg;
    TextMeshProUGUI label;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        label = GetComponent<TextMeshProUGUI>();
    }

    public void Init(string txt)
    {
        label.text = txt;
        transform.SetAsLastSibling();  // draw on top

        // optional tweak: color by amount
        float value = float.Parse(txt.Replace(" MW", ""));
        label.color = value >= 2f ? Color.green : new Color(1f, 0.5f, 0f);

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float t = 0f;
        Vector3 start = transform.localPosition;
        Vector3 end = start + Vector3.up * 50f;  // rise

        while (t < lifeSeconds)
        {
            t += Time.unscaledDeltaTime;
            float p = t / lifeSeconds;
            cg.alpha = 1f - p;
            transform.localPosition = Vector3.Lerp(start, end, p);
            yield return null;
        }
        Destroy(gameObject);
    }
}
