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

    private void Start()
    {
        StartMusic();
    }


    public void StartMusic()
    {
        _backgroundAudioSource.volume = 1;
        _backgroundAudioSource.PlayScheduled(AudioSettings.dspTime + .5);
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

        SwitchTrack(fromIndex, toIndex);
    }

    public void SwitchTrack(int from, int to)
    {
        StartCoroutine(AudioManager.CrossFadeCoroutine(_characterAudioSources[from], _characterAudioSources[to], 1.5f, 0f, 1f));
    }
}
