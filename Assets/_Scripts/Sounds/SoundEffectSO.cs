using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "ScriptableObjects/Sounds/SoundEffect")]
public class SoundEffectSO : ScriptableObject
{
    [Header("Sound Effect Details")]
    [Tooltip("The name for the sound effect")]
    public string soundEffectName;

    [Tooltip("The prefab for the sound effect")]
    public GameObject soundPrefab;

    [Tooltip("The audio clip for the sound effect")]
    public AudioClip soundEffectClip;

    [Tooltip("The minimum pitch variation for the sound effect.")]
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMin = 0.8f;

    [Tooltip("The maximum pitch variation for the sound effect.")]
    [Range(0.1f, 1.5f)]
    public float soundEffectPitchRandomVariationMax = 1.2f;

    [Tooltip("The sound effect volume.")]
    [Range(0f, 1f)]
    public float soundEffectVolume = 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        ValidateProperties();
    }

    private void ValidateProperties()
    {
        ValidateNotEmptyString(nameof(soundEffectName), soundEffectName);
        ValidateNotNull(nameof(soundPrefab), soundPrefab);
        ValidateNotNull(nameof(soundEffectClip), soundEffectClip);
        ValidateRange(nameof(soundEffectPitchRandomVariationMin), soundEffectPitchRandomVariationMin, nameof(soundEffectPitchRandomVariationMax), soundEffectPitchRandomVariationMax);
        ValidatePositiveValue(nameof(soundEffectVolume), soundEffectVolume);
    }

    private void ValidateNotEmptyString(string propertyName, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError($"Property '{propertyName}' cannot be empty or null.", this);
        }
    }

    private void ValidateNotNull<T>(string propertyName, T value) where T : Object
    {
        if (value == null)
        {
            Debug.LogError($"Property '{propertyName}' cannot be null.", this);
        }
    }

    private void ValidateRange(string minPropertyName, float minValue, string maxPropertyName, float maxValue)
    {
        if (minValue > maxValue)
        {
            Debug.LogError($"Property '{minPropertyName}' must be less than or equal to '{maxPropertyName}'.", this);
        }
    }

    private void ValidatePositiveValue(string propertyName, float value)
    {
        if (value < 0)
        {
            Debug.LogError($"Property '{propertyName}' must be a positive value.", this);
        }
    }
#endif
}
