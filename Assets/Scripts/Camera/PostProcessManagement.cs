using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


public class PostProcessManagement : MonoBehaviour
{
    private PostProcessVolume _volume;
    private DepthOfField _depthField;

    [SerializeField]
    private CharacterMovement _character;

    [SerializeField]
    private float minDistance = .2f;
    [SerializeField]
    private float maxDistance = 3.5f;

    [SerializeField]
    private float minFocal = .1f;
    [SerializeField]
    private float maxFocal = .8f;

    [SerializeField]
    private CameraObstacleDetection _camera;
    private float _focusDistance;

    private void Awake()
    {
        _volume = GetComponent<PostProcessVolume>();
        _volume.profile = Instantiate(_volume.sharedProfile);
        _depthField = _volume.profile.GetSetting<DepthOfField>();
    }

    private void Update()
    {
        if(_character._movementType == MovementType.CrawlingMovement || _character._movementType == MovementType.JumpingMovement)
        {
            _focusDistance = _camera._currentDistance;
            float lerp = _focusDistance / maxDistance - minDistance;

            float lerpValue = Mathf.Lerp(maxFocal, minFocal, lerp);
            _depthField.focusDistance.value = _focusDistance;

            _depthField.aperture.value = lerpValue;
        }
        else
        {
            _depthField.focusDistance.value = 2;
            _depthField.aperture.value = 1;
        }

       


    }
}
