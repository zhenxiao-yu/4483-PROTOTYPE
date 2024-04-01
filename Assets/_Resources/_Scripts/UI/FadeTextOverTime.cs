using System.Collections;
using UnityEngine;
using TMPro; // Add this for TextMeshPro support

public class FadeTextOverTime : MonoBehaviour
{
    public TMP_Text textToFade; // Assign this in the inspector
    public float delayBeforeFadeStarts = 30f; // Time in seconds before fading starts
    public float fadeDuration = 2f; // Duration of the fade

    private void Start()
    {
        // Start the FadeOut coroutine
        StartCoroutine(FadeOutText());
    }

    private IEnumerator FadeOutText()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeFadeStarts);

        // Fade out the text over the fadeDuration
        float startTime = Time.time;
        Color startColor = textToFade.color;
        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            textToFade.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0, t));
            yield return null; // Wait for a frame
        }

        // Ensure the text is fully transparent after fading
        textToFade.color = new Color(startColor.r, startColor.g, startColor.b, 0);
    }
}
