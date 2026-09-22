using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextSequenceAnimator : MonoBehaviour
{    
    [Header("UI Components")]
    public Text textLabel;

    [Header("Text Settings")]
    public List<string> texts;            // List of strings to show
    public float fadeDuration = 0.5f;     // Time to fade in/out
    public float displayDuration = 1f;    // Time text stays fully visible
    public int startFontSize = 20;
    public int endFontSize = 40;

    [Header("Optional")]
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public UnityEvent onSequenceFinished;

    private void Start()
    {
        if (textLabel != null && texts.Count > 0)
            StartCoroutine(AnimateTexts());
    }

    private IEnumerator AnimateTexts()
    {
        yield return new WaitForSeconds(0.2f);
        CanvasGroup canvasGroup = textLabel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = textLabel.gameObject.AddComponent<CanvasGroup>();
        }

        foreach (string txt in texts)
        {
            textLabel.text = txt.Replace("\\n", "\n");

            // Fade in + scale up
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float normalized = Mathf.Clamp01(t / fadeDuration);

                canvasGroup.alpha = normalized; // fade
                textLabel.fontSize = Mathf.RoundToInt(Mathf.Lerp(startFontSize, endFontSize, scaleCurve.Evaluate(normalized)));

                yield return null;
            }

            canvasGroup.alpha = 1f;
            textLabel.fontSize = endFontSize;

            // Stay visible
            yield return new WaitForSeconds(displayDuration);

            // Fade out + scale down
            t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float normalized = Mathf.Clamp01(t / fadeDuration);

                canvasGroup.alpha = 1 - normalized;
                textLabel.fontSize = Mathf.RoundToInt(Mathf.Lerp(endFontSize, startFontSize, scaleCurve.Evaluate(normalized)));

                yield return null;
            }

            canvasGroup.alpha = 0f;
            textLabel.fontSize = startFontSize;
        }
        onSequenceFinished.Invoke();
    }
}
