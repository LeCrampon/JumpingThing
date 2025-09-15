using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FlyingAudio : MonoBehaviour
{

    public Coroutine _fadeCoroutine;
    public AudioMixerGroup _audioMixerGroup;

    public void StartFlyingAudio()
    {
        ResetCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(AudioManager.FadeCoroutine(_audioMixerGroup.audioMixer, "FlyingVolumeParam", .1f, 1f));
    }

    public void StopFlyingAudio()
    {
        ResetCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(AudioManager.FadeCoroutine(_audioMixerGroup.audioMixer, "FlyingVolumeParam", .1f, 0f));
    }

    public void ResetCoroutine(Coroutine cor)
    {
        if (cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }
    }
}
