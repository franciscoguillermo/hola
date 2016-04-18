using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    private Shapeshifter _player;

    public AudioClip Acoustics;
    public AudioClip Bass;
    public AudioClip Drums;
    public AudioClip Strings;
    public AudioClip Wind;
    public AudioClip Splash;
    public AudioClip RingChime;

    public GameObject PooledAudioSourcePrefab;

    private AudioSource[] _audioSources;

    private float _activeFormVolume = 1.0f;
    private float _passiveFormVolume = 0.25f;
    private float _crossFadeSmoothFactor = 5.0f;

    public static void PlaySplash(float volume, float pitch)
    {
        ObjectPool.Pop<PooledAudioSource>(_instance.PooledAudioSourcePrefab).Play(_instance.Splash, volume, pitch);
    }

    public static void PlayRingChime()
    {
        ObjectPool.Pop<PooledAudioSource>(_instance.PooledAudioSourcePrefab).Play(_instance.RingChime, 1, 1);
    }

    public void OnLevelWasLoaded(int level)
    {
        if (this != _instance)
            return;

        _player = FindObjectOfType<Shapeshifter>();
        
        for (int i = 0; i < _audioSources.Length; i++)
            _audioSources[i].pitch = 1;

        _audioSources[4].volume = 0;
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _player = FindObjectOfType<Shapeshifter>();

        _audioSources = GetComponents<AudioSource>();

        _audioSources[0].clip = Acoustics;
        _audioSources[1].clip = Bass;
        _audioSources[2].clip = Drums;
        _audioSources[3].clip = Strings;
        _audioSources[4].clip = Wind;

        for (int i = 0; i < _audioSources.Length; i++)
            _audioSources[i].Play();

        _audioSources[4].volume = 0;
    }

    void Update()
    {
        _audioSources[0].volume = Mathf.Lerp(_audioSources[0].volume, _player.Form == AnimalForms.Fox ? _activeFormVolume : _passiveFormVolume, Time.deltaTime * _crossFadeSmoothFactor);
        _audioSources[1].volume = Mathf.Lerp(_audioSources[1].volume, _player.Form == AnimalForms.Fish ? _activeFormVolume : _passiveFormVolume, Time.deltaTime * _crossFadeSmoothFactor);
        _audioSources[2].volume = Mathf.Lerp(_audioSources[2].volume, 1, Time.deltaTime * _crossFadeSmoothFactor);
        _audioSources[3].volume = Mathf.Lerp(_audioSources[3].volume, _player.Form == AnimalForms.Bird ? _activeFormVolume : _passiveFormVolume, Time.deltaTime * _crossFadeSmoothFactor);
    }

    public static void DetuneDeath()
    {
        _instance.StartCoroutine(_instance.DetuneDeathAsync());
    }

    IEnumerator DetuneDeathAsync()
    {
        _audioSources[4].volume = 1;

        for (float t = 0; t < 0.5; t += Time.deltaTime)
        {
            float normalT = t / 0.5f;

            for (int i = 0; i < 4; i++)
            {
                _audioSources[i].pitch = 1 - normalT;
                _audioSources[i].volume = 1 - normalT;
            }
            yield return null;
        }
    }
}