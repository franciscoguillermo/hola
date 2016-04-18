using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PooledAudioSource : MonoBehaviour
{
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, float volume, float pitch)
    {
        gameObject.SetActive(true);
        _audioSource.clip = clip;
        _audioSource.volume = volume;
        _audioSource.pitch = pitch;
        _audioSource.Play();
    }
}