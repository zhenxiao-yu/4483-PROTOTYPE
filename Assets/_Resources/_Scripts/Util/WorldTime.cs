using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class WorldTime : MonoBehaviour
{
    public float duration = 5f;

    [SerializeField] private Gradient gradient;
    // Reference to the Volume component
    [SerializeField] private Volume postProcessVolume;
    private Light2D _light;
    private float _startTime;
    private Vignette _vignette;
    private LensDistortion _lensDistortion; // For lens distortion
    private Bloom _bloom; // Added for bloom

    private ColorAdjustments _colorAdjustments; // Added for color adjustments

    private bool hallucinationEffectActive = false;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
        _startTime = Time.time;

        // Try to get the Vignette, LensDistortion, and Bloom components from the Volume
        if (postProcessVolume.profile.TryGet(out Vignette vignette))
        {
            _vignette = vignette;
        }
        if (postProcessVolume.profile.TryGet(out LensDistortion lensDistortion))
        {
            _lensDistortion = lensDistortion;
        }
        if (postProcessVolume.profile.TryGet(out Bloom bloom)) // Added for bloom
        {
            _bloom = bloom;
        }
        if (postProcessVolume.profile.TryGet(out ColorAdjustments colorAdjustments)) // Added for color adjustments
        {
            _colorAdjustments = colorAdjustments;
        }
    }

    // Method to activate the hallucination effect
    public void ActivateHallucinationEffect(float duration)
    {
        if (!hallucinationEffectActive)
        {
            StartCoroutine(HallucinationEffectDuration(duration));
        }
    }
    private IEnumerator HallucinationEffectDuration(float duration)
    {
        hallucinationEffectActive = true;

        // Capture initial states before changes
        Color originalLightColor = _light.color;
        float originalVignetteIntensity = _vignette != null ? _vignette.intensity.value : 0f;
        float originalLensDistortionIntensity = _lensDistortion != null ? _lensDistortion.intensity.value : 0f;
        float originalLensDistortionScale = _lensDistortion != null ? _lensDistortion.scale.value : 1f;
        float originalBloomIntensity = _bloom != null ? _bloom.intensity.value : 0f;
        float originalHueShift = _colorAdjustments != null ? _colorAdjustments.hueShift.value : 0f;

        // Wait for the duration of the hallucination effect
        yield return new WaitForSeconds(duration);

        // Reset post-processing effects to their original states
        _light.color = originalLightColor;
        if (_vignette != null) _vignette.intensity.value = originalVignetteIntensity;
        if (_lensDistortion != null)
        {
            _lensDistortion.intensity.value = originalLensDistortionIntensity;
            _lensDistortion.scale.value = originalLensDistortionScale;
        }
        if (_bloom != null) _bloom.intensity.value = originalBloomIntensity;
        if (_colorAdjustments != null) _colorAdjustments.hueShift.value = originalHueShift;

        hallucinationEffectActive = false;
    }


    void Update()
    {
        if (!hallucinationEffectActive) return;
        var timeElapsed = Time.time - _startTime;
        var percentage = Mathf.Sin(timeElapsed / duration * Mathf.PI * 2) * 0.5f + 0.5f;

        percentage = Mathf.Clamp01(percentage);
        _light.color = gradient.Evaluate(percentage);
        if (_vignette != null)
        {
            _vignette.intensity.value = 0.4f + (percentage * 0.65f); 
        }
        // Simulate hallucination effect using lens distortion and bloom
        if (_lensDistortion != null)
        {
            _lensDistortion.intensity.value = Mathf.PingPong(Time.time * 0.5f, 1) - 0.5f; // Oscillate intensity
            _lensDistortion.scale.value = 1 - (Mathf.PingPong(Time.time * 0.1f, 0.2f)); // Oscillate scale
        }

        if (_bloom != null)
        {
            _bloom.intensity.value = 2 + Mathf.PingPong(Time.time, 2); // Oscillate bloom intensity 
        }

        // Attempt to create a dynamic color shift for a surreal effect
        if (_colorAdjustments != null)
        {
            _colorAdjustments.hueShift.value = Mathf.PingPong(Time.time * 40, 180) - 90; // shift hue over time
        }
    }
}
