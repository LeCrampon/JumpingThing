using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource[] _characterAudioSources;
    [SerializeField]
    private AudioSource _backgroundAudioSource;
    [SerializeField]
    private CharacterMovement[] _characters;
    [SerializeField]
    private AudioSource _sitarAudioSource;

    private AudioSource _currentAudioSource;

    private void Start()
    {
        StartMusic();
    }


    public void StartMusic()
    {
        _backgroundAudioSource.volume = 1;
        _backgroundAudioSource.PlayScheduled(AudioSettings.dspTime + .5);
        _sitarAudioSource.volume = 0;
        _sitarAudioSource.PlayScheduled(AudioSettings.dspTime + .5);
        foreach (AudioSource source in _characterAudioSources)
        {
            source.volume = 0;
            source.PlayScheduled(AudioSettings.dspTime + .5);
        }
    }

    public void SwitchCharacterTrack(CharacterMovement from, CharacterMovement to)
    {
        int fromIndex = 0;
        int toIndex = 0;
        for(int i =0; i< _characters.Length; i++)
        {
            if (from == _characters[i])
            {
                fromIndex = i; 
            }
            if (to == _characters[i])
            {
                toIndex = i;
            }
        }

        AudioSource previousAS = _characterAudioSources[fromIndex];
        AudioSource nextAS = _characterAudioSources[toIndex];
        if (from != null && from._isPoisoned)
        {
            previousAS = _sitarAudioSource;
        }

        if (to._isPoisoned)
        {
            nextAS = _sitarAudioSource;
        }

        SwitchTrack(previousAS, nextAS);
    }

    public void SwitchToPoisonedTrack(CharacterMovement from)
    {
        int fromIndex = 0;
        for (int i = 0; i < _characters.Length; i++)
        {
            if (from == _characters[i])
            {
                fromIndex = i;
            }
        }
        if(_sitarAudioSource != _currentAudioSource)
            SwitchTrack(_characterAudioSources[fromIndex], _sitarAudioSource);
    }

    public void SwitchFromPoisonedTrack(CharacterMovement to)
    {
        int toIndex = 0;
        for (int i = 0; i < _characters.Length; i++)
        {
            if (to == _characters[i])
            {
                toIndex = i;
            }
        }
        if (_characterAudioSources[toIndex] != _currentAudioSource)
            SwitchTrack(_sitarAudioSource, _characterAudioSources[toIndex]);
    }

    public void SwitchTrack(AudioSource from, AudioSource to)
    {
        StartCoroutine(AudioManager.CrossFadeCoroutine(from, to, 1.5f, 0f, 1f));
        _currentAudioSource = to;
    }
}
