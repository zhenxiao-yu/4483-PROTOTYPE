using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioData
{
    [SerializeField] private string audioName;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] private float minPitch = 0.9f;
    [SerializeField] private float maxPitch = 1.1f;

    // Getters for private fields
    public string AudioName => audioName;
    //public AudioClip GetAudioClip => audioClips[Random.Range(0, audioClips.Length)];
    //public float GetPitch => Random.Range(minPitch, maxPitch);
}