using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


public class PostProcessManagement : MonoBehaviour
{
    private PostProcessVolume _volume;
    private DepthOfField _depthField;
    private ColorGrading _colorGrading;

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

    [SerializeField]
    private PostProcessData _crawlingData, _jumpingData, _flyingData;
    [SerializeField]
    private CharacterMovement _crawlingCharacter, _jumpingCharacter, _flyingCharacter;
    private bool _isPoisoned;

    private Coroutine _poisonedCoroutine;

    private void Awake()
    {
        _volume = GetComponent<PostProcessVolume>();
        _volume.profile = Instantiate(_volume.sharedProfile);
        _depthField = _volume.profile.GetSetting<DepthOfField>();
        _colorGrading = _volume.profile.GetSetting<ColorGrading>();
        _colorGrading.ldrLutContribution.value = 0f;
    }

    public void SetCamera(CameraObstacleDetection camera)
    {
        _camera = camera;
    }

    private void Update()
    {
        if(_character._movementType == MovementType.CrawlingMovement || _character._movementType == MovementType.JumpingMovement)
        {
            _focusDistance = _camera._currentDistance;
            float lerp = _focusDistance / maxDistance - minDistance;

            //float lerpValue = Mathf.Lerp(maxFocal, minFocal, lerp);
            _depthField.focusDistance.value = _focusDistance;

            //_depthField.aperture.value = lerpValue;
            _depthField.aperture.value = 4.5f;
        }
        else
        {
            _focusDistance = _camera._currentDistance;
            _depthField.focusDistance.value = 1.9f;

            float lerp = _focusDistance / maxDistance - minDistance;

            //_depthField.aperture.value = Mathf.Lerp(maxFocal, minFocal, lerp);
            _depthField.aperture.value = 4.5f;
        }

    }

    public void LoadData(PostProcessData data)
    {
        minDistance = data.minDistance;
        maxDistance = data.maxDistance;
        minFocal = data.minFocal;
        maxFocal = data.maxFocal;
    }

    public void SwitchPostProcessValues(CharacterMovement character)
    {
        _character = character;
        if (character == _crawlingCharacter)
        {
            LoadData(_crawlingData);
        }
        else if (character == _jumpingCharacter)
        {
            LoadData(_jumpingData);
        }
        else if (character == _flyingCharacter)
        {
            LoadData(_flyingData);
        }
    }

    public void StartPoisoning()
    {
        if (!_isPoisoned)
        {
            if(_poisonedCoroutine != null)
                StopCoroutine(_poisonedCoroutine);
            _poisonedCoroutine = StartCoroutine(StartPoisoningCoroutine());
            _isPoisoned = true;
        }

    }

    private IEnumerator StartPoisoningCoroutine()
    {
        while(_colorGrading.ldrLutContribution.value < 1)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            _colorGrading.ldrLutContribution.value += .01f;
        }

    }

    public void StopPoisoning()
    {
        if (_isPoisoned)
        {
            if (_poisonedCoroutine != null)
                StopCoroutine(_poisonedCoroutine);
            _poisonedCoroutine = StartCoroutine(StopPoisoningCoroutine());
            _isPoisoned = false;
        }

    }

    private IEnumerator StopPoisoningCoroutine()
    {
        while (_colorGrading.ldrLutContribution.value > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            _colorGrading.ldrLutContribution.value -= .01f;
        }

    }
}
