using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioManager { 
    //public static AudioManager _instance;

    //private void Awake()
    //{
    //    if(_instance != this && _instance != null)
    //    {
    //        Destroy(this.gameObject);
    //    }
    //    else
    //    {
    //        _instance = this;
    //    }
    //}

    public static IEnumerator FadeCoroutineMixer(AudioMixer mixer, string mixerParam, float seconds, float targetVolume )
    {
        float currentTime = 0f;
        float currentVolume;
        mixer.GetFloat(mixerParam, out currentVolume);
        //Decibel to linéaire
        currentVolume = Mathf.Pow(10, currentVolume / 20);
        //Clamp by safety
        targetVolume = Mathf.Clamp(targetVolume, 0.00001f, 1f);

        while(currentTime < seconds)
        {
            currentTime += Time.deltaTime;
            float volume = Mathf.Lerp(currentVolume, targetVolume, currentTime / seconds);
            //On reconvertit de linéaire vers Decibel (logarithmique)
            mixer.SetFloat(mixerParam, Mathf.Log10(volume) * 20);
            yield return null;
        }
    }

    public static IEnumerator FadeCoroutine(AudioSource from, float seconds, float targetVolume)
    {
        float currentTime = 0f;
        float currentVolume = from.volume;

        while (currentTime < seconds)
        {
            currentTime += Time.deltaTime;
            float volume = Mathf.Lerp(currentVolume, targetVolume, currentTime / seconds);
            from.volume = volume;
            yield return null;
        }
    }

    public static IEnumerator CrossFadeCoroutine(AudioSource from, AudioSource to, float seconds, float targetFromVolume, float targetToVolume)
    {
        float currentTime = 0f;
        float currentFromVolume = from.volume;
        float currentToVolume = to.volume;

        while (currentTime < seconds)
        {
            currentTime += Time.deltaTime;
            float fromVolume = Mathf.Lerp(currentFromVolume, targetFromVolume, currentTime / seconds);
            float toVolume = Mathf.Lerp(currentToVolume, targetToVolume, currentTime / seconds);
            //On reconvertit de linéaire vers Decibel (logarithmique)
            from.volume = fromVolume;
            to.volume = toVolume;
            yield return null;
        }
    }
}
