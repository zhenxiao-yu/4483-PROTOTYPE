using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "ScriptableObjects/Sounds/MusicTrack")]
public class MusicTrackSO : ScriptableObject
{
    [Header("Music Track Details")]
    [Tooltip("The name for the music track")]
    public string musicName;

    [Tooltip("The audio clip for the music track")]
    public AudioClip musicClip;

    [Tooltip("The volume for the music track")]
    [Range(0, 1)]
    public float musicVolume = 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        ValidateProperties();
    }

    private void ValidateProperties()
    {
        ValidateNotEmptyString(nameof(musicName), musicName);
        ValidateNotNull(nameof(musicClip), musicClip);
        ValidatePositiveValue(nameof(musicVolume), musicVolume);
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

    private void ValidatePositiveValue(string propertyName, float value)
    {
        if (value < 0)
        {
            Debug.LogError($"Property '{propertyName}' must be a positive value.", this);
        }
    }
#endif
}
