using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FootStepAudio : MonoBehaviour
{
    [SerializeField]
    private CharacterMovement _characterMovement;

    [SerializeField]
    private AudioMixerGroup _audioMixerGroup;
    private Coroutine _fadeCoroutine;
    private Coroutine _switchCoroutine;

    [SerializeField]
    private AudioSource _grassSource, _defaultSource, _glassSource;

    private AudioSource _currentSource;

    [Header("RayCast")]
    [SerializeField]
    private Transform _rayCastStartPos;
    [SerializeField]
    private LayerMask _layerMask;

    private Material mat;

    [SerializeField]
    private GroundType _currentGroundType;

    private void Awake()
    {
        _currentSource = _defaultSource;
    }

    private void Update()
    {
        if(_characterMovement._movementType == MovementType.CrawlingMovement)
        {
            CheckGroundType();
        }

    }

    public void PlayJumpAudio()
    {

    }

    public void StartFootStepAudio()
    {
        ResetCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(AudioManager.FadeCoroutine(_audioMixerGroup.audioMixer, "VolumeParam", .1f, 1f));
    }

    public void StopFootStepAudio()
    {
        ResetCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(AudioManager.FadeCoroutine(_audioMixerGroup.audioMixer, "VolumeParam", .1f, 0f));
    }

    public void CheckGroundType()
    {
        //TODO:
        //CHANGE THE MESH OF THE GROUND, TO HAVE HOLES, GRASS and UNDER THE GRASS
        //Grass: MeshCollider
        //UnderTheGRass: no mesh Collider
        //THEN, Only Get Material once, it will be easier and less costly

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(_rayCastStartPos.position, -_rayCastStartPos.up, out hit, 1f, _layerMask))
        {
            Renderer rend = hit.collider.GetComponent<Renderer>();
            if (rend != null)
            {
                mat = rend.sharedMaterial;
                //Debug.Log("Walking on " + mat.name);
            }
        }
        if (mat == null)
            return;

        GroundType newGroundType;
        switch (mat.name)
        {
            case "Soil_MAT":
                newGroundType = GroundType.Default;
                break;
            case "grass_MAT":
                newGroundType = GroundType.Grass;
                break;
            case "Glass_MAT":
                newGroundType = GroundType.Glass;
                break;
            default:
                newGroundType = GroundType.Default;
                break;
        }

        if(_currentGroundType != newGroundType)
        {
            SwitchFootStepAudio(newGroundType);
            _currentGroundType = newGroundType;
        }
    }

    public void SwitchFootStepAudio(GroundType type)
    {
        AudioSource _nextSource;
        switch (type)
        {
            case GroundType.Default:
                _nextSource = _defaultSource;
                break;
            case GroundType.Grass:
                _nextSource = _grassSource;
                break;
            case GroundType.Glass:
                _nextSource = _glassSource;
                break;
            default:
                _nextSource = _defaultSource;
                break;
        }
        StartCoroutine(AudioManager.CrossFadeCoroutine(_currentSource, _nextSource, .1f, 0f, 1f));
        _currentSource = _nextSource;
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

public enum GroundType
{
    Default,
    Grass,
    Glass
}
