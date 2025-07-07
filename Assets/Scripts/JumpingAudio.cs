using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingAudio : MonoBehaviour
{

    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _audioClip;

    public void PlayJumpingAudio()
    {
        _audioSource.pitch = Random.Range(0.8f, 1.4f);
        _audioSource.PlayOneShot(_audioClip);
    }
}
