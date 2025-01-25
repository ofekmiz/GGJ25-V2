using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private int _audioSourceCount = 5;
    [SerializeField] private List<AudioData> _sounds;
    [SerializeField] private AudioData _bgSound;
    
    private float _audioEaseTime = 0.8f;
    
    private AudioSourceData[] _gameAudioSources;
    private AudioSourceData _bgAudioSource;

    private int _currentAudioSourceIdx = 0;
    
    private AudioPlayer _bgAudioPlayer;
    
    public struct AudioSourceData
    {
        public AudioSource AudioSource;
        public AudioData AudioData;
    }
    
    [Serializable]
    public struct AudioData
    {
        public string Name;
        public List<AudioClip> Clips;
        public float Pitch;
        public float RandomPitch;
        [Range(0f, 1f)] public float Volume;
    }

    private void Awake()
    {
        Init();
        PlayBgAudio();
    }

    public void Init()
    {
        _gameAudioSources = new AudioSourceData[_audioSourceCount];
        for (int i = 0; i < _audioSourceCount; ++i)
        {
            AudioSourceData audioSourceData = new AudioSourceData();
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSourceData.AudioSource = audioSource;
            _gameAudioSources[i] = audioSourceData;
        }

        AudioSource bgAudioSource = gameObject.AddComponent<AudioSource>();
        bgAudioSource.playOnAwake = false;
        bgAudioSource.loop = true;
        setAudioSource(bgAudioSource, _bgSound);
        _bgAudioPlayer = new AudioPlayer(bgAudioSource, this);

        preLoadSounds();
    }
    
    public void PlayBgAudio()
    {
        _bgAudioPlayer.AudioSource.Play();
        _bgAudioPlayer.SetVolume(_bgSound.Volume);
        _bgAudioPlayer.SmoothVolume(0, 1, _audioEaseTime);
    }

    public void StopBgAudio()
    {
        _bgAudioPlayer.SmoothVolume(1, 0, _audioEaseTime);
    }
    
    public void PlayGameAudio(string audioName)
    {
        AudioData audioData = getAudioData(audioName);
        
        AudioSourceData audioSourceData = _gameAudioSources[_currentAudioSourceIdx];
        _currentAudioSourceIdx = (_currentAudioSourceIdx + 1) % _gameAudioSources.Length;
        setAudioSource(audioSourceData.AudioSource, audioData);

        audioSourceData.AudioData = audioData;
        audioSourceData.AudioSource.time = 0;
        audioSourceData.AudioSource.volume = audioData.Volume;
        audioSourceData.AudioSource.Play();
    }

    private AudioData getAudioData(string audioName)
    {
        for (int i = 0; i < _sounds.Count; ++i)
            if (_sounds[i].Name == audioName)
                return _sounds[i];

        
        Debug.LogError($"Could not find sound with name {audioName}");
        return _sounds[0];
    }
    
    private void preLoadSounds()
    {
        foreach (AudioData audioData in _sounds)
        {
            foreach (var clip in audioData.Clips)
                clip.LoadAudioData();   
        }
    }
    
    private void setAudioSource(AudioSource audioSource, AudioData audioData)
    {
        if (audioData.Clips.Count == 1)
            audioSource.clip = audioData.Clips[0];
        else
            audioSource.clip = audioData.Clips[Random.Range(0, audioData.Clips.Count)];
            
        audioSource.pitch = 1;
        if (audioData.RandomPitch > 0)
        {
            float randomPitch = Random.Range(-audioData.RandomPitch, audioData.RandomPitch);
            audioSource.pitch += randomPitch;
        }
    }
}

public class AudioPlayer
{
    public AudioSource AudioSource;

    private AudioManager _audioManager;

    private float _volumeMultiplier;
    private float _audioVolume;

    private Coroutine _volumeCoroutine;
        
    public AudioPlayer(AudioSource audioSource, AudioManager audioManager)
    {
        AudioSource = audioSource;
        _audioManager = audioManager;
    }

    public void SetVolume(float volume)
    {
        _audioVolume = volume;
        updateVolume();
    }

    public void SmoothVolume(float startMultiplier, float endMultiplier, float time)
    {
        if (_volumeCoroutine != null)
            _audioManager.StopCoroutine(_volumeCoroutine);
            
        _volumeCoroutine = _audioManager.StartCoroutine(smoothVolumeCo(startMultiplier, endMultiplier, time));
    }

    private IEnumerator smoothVolumeCo(float startMultiplier, float endMultiplier, float totalTime)
    {
        float currTime = 0;
        while (currTime < totalTime)
        {
            float percent = currTime / totalTime;
            _volumeMultiplier = Mathf.Lerp(startMultiplier, endMultiplier, percent);
            updateVolume();    
                
            currTime += Time.deltaTime;
            yield return null;
        }

        _volumeMultiplier = endMultiplier;
        updateVolume();
            
        if (endMultiplier == 0)
            AudioSource.Stop();
    }

    private void updateVolume()
    {
        AudioSource.volume = _volumeMultiplier * _audioVolume;
    }
}