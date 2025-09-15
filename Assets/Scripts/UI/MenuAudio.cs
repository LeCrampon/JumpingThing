using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _hovered, _selected;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayMenuHovered()
    {
        _audioSource.PlayOneShot(_hovered);
    }

    public void PlayMenuSelected()
    {
        _audioSource.PlayOneShot(_selected);
    }
}
