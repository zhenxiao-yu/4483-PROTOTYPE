using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Create custom extensions
public static class Extensions
{
    // Execute an action after a specified delay
    public static void DelayedAction(this MonoBehaviour mb, System.Action action, float delay)
    {
        mb.StartCoroutine(DelayedCoroutine(action, delay));
    }

    // Coroutine for delayed action
    static IEnumerator DelayedCoroutine(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Invoke the action if it's not null
        action?.Invoke();
    }

    // Get a position inside the screen while considering a specified offset
    public static Vector3 GetPositionInsideScreen(Vector2 baseRes, RectTransform rect, float offset)
    {
        float widthBounds = baseRes.x - rect.rect.width - offset;
        float heightBounds = baseRes.y - rect.rect.height - offset;

        Vector2 adjustedPos = rect.anchoredPosition;

        // Ensure the position is within the screen bounds
        adjustedPos.x = Mathf.Clamp(adjustedPos.x, widthBounds * -0.5f, widthBounds * 0.5f);
        adjustedPos.y = Mathf.Clamp(adjustedPos.y, heightBounds * -0.5f, heightBounds * 0.5f);

        return adjustedPos;
    }

    // Play an audio clip from an AudioData object
    public static void PlayAudioData(this AudioSource aSource, AudioData audioData)
    {
        //aSource.pitch = audioData.GetPitch;
        //aSource.clip = audioData.GetAudioClip;
        //aSource.Play();
    }
}