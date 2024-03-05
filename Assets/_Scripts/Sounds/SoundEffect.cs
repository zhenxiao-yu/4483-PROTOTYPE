using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class SoundEffect : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        InitializeAudioSource();
    }

    private void InitializeAudioSource()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found.", this);
        }
    }

    private void OnEnable()
    {
        PlayAudioClip();
    }

    private void OnDisable()
    {
        StopAudioClip();
    }

    private void PlayAudioClip()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void StopAudioClip()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void SetSound(SoundEffectSO soundEffect)
    {
        if (audioSource == null)
        {
            InitializeAudioSource();
        }

        if (soundEffect == null)
        {
            Debug.LogError("SoundEffectSO is null.", this);
            return;
        }

        audioSource.pitch = Random.Range(soundEffect.soundEffectPitchRandomVariationMin, soundEffect.soundEffectPitchRandomVariationMax);
        audioSource.volume = soundEffect.soundEffectVolume;
        audioSource.clip = soundEffect.soundEffectClip;
    }
}
