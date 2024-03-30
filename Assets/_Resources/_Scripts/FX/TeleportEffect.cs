using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    public IEnumerator MaterializeRoutine(Shader materializeShader, Color materializeColor, float materializeTime, SpriteRenderer[] spriteRendererArray, Material normalMaterial)
    {
        Material materializeMaterial = new Material(materializeShader);

        materializeMaterial.SetColor("_EmissionColor", materializeColor);
        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = materializeMaterial;
        }

        float dissolveAmount = 0f;
        // Initialize pulseIntensity and a boolean to control the direction of the pulse
        float pulseIntensity = 1f;
        bool increasingIntensity = true;

        while (dissolveAmount < 1f)
        {
            dissolveAmount += Time.deltaTime / materializeTime;
            materializeMaterial.SetFloat("_DissolveAmount", dissolveAmount);

            // Pulse logic
            if (increasingIntensity)
            {
                pulseIntensity += Time.deltaTime; // Increase intensity
                if (pulseIntensity >= 2) // Example peak intensity
                {
                    increasingIntensity = false; // Reverse direction at peak
                }
            }
            else
            {
                pulseIntensity -= Time.deltaTime; // Decrease intensity
                if (pulseIntensity <= 1) // Return to base intensity
                {
                    increasingIntensity = true; // Reverse direction at base
                }
            }
            materializeMaterial.SetFloat("_PulseIntensity", pulseIntensity);

            yield return null;
        }

        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = normalMaterial;
        }
    }
}
