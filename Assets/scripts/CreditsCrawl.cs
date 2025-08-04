using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CreditsCrawl : MonoBehaviour
{
    [SerializeField] RectTransform contentRT;   // CreditsText RectTransform
    [SerializeField] float crawlDuration = 60f; // seconds to reach top
    [SerializeField] float startDelay = 2f;     // seconds before movement

    Vector2 startPos, endPos;

    void Start()
    {
        startPos = contentRT.anchoredPosition;
        // viewport height = 800, content height from rect
        float totalHeight = contentRT.rect.height + 800;
        endPos = startPos + Vector2.up * totalHeight;
        StartCoroutine(DoCrawl());
    }

    System.Collections.IEnumerator DoCrawl()
    {
        yield return new WaitForSecondsRealtime(startDelay);
        float t = 0f;
        while (t < crawlDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / crawlDuration;
            contentRT.anchoredPosition = Vector2.Lerp(startPos, endPos, p);
            yield return null;
        }
        SceneManager.LoadScene("MainMenu");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");
    }
}
