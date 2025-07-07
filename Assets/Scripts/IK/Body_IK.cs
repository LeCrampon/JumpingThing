using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_IK : MonoBehaviour
{
    [SerializeField]
    private Transform _headTransform;
    [SerializeField]
    private Transform _hipsTransform;
    [SerializeField]
    private float _headStepLerp = 0;
    [SerializeField]
    private float _headLerpSpeed;

    [SerializeField]
    private float _maxRotation;

    private Vector3 _startHipPos;
    [SerializeField]
    private float _hipsStepLerp = 0;
    [SerializeField]
    private float _hipsLerpSpeed;
    [SerializeField]
    private float _hipsMovementAmplitude;

    [SerializeField]
    private Camera _camera;

    private void Awake()
    {
        _startHipPos = _hipsTransform.localPosition;
    }

    public void BobHead()
    {
        if (_headStepLerp >= 1)
        {
            _headStepLerp = 0;
        }
        Quaternion newRotation = Quaternion.Euler(Mathf.Sin(_headStepLerp * Mathf.PI) * _maxRotation, transform.localRotation.y, transform.localRotation.z );
        _headStepLerp += Time.deltaTime * _headLerpSpeed;


        _headTransform.localRotation = newRotation;
    }

    public void RotateHead(Vector3 cameraForward)
    {
        //Vector3 camForwardProj = Vector3.ProjectOnPlane(cameraForward, up).normalized;
        //Vector3 calculatedRight = Vector3.Cross(up, cameraForward).normalized;
        Vector3 target = _headTransform.position + cameraForward * 10f;
        Debug.Log("ROTATING");
        _headTransform.rotation = Quaternion.LookRotation(target, _headTransform.up);
    }

    public void ThoseHipsDontLie()
    {
        if (_hipsStepLerp >= 1) { 
            _hipsStepLerp = 0;
        }
        float newY = Mathf.Sin(_hipsStepLerp * Mathf.PI) * _hipsMovementAmplitude;
        _hipsTransform.localPosition = new Vector3(transform.localPosition.x, _startHipPos.y + newY, transform.localPosition.z);
        _hipsStepLerp += Time.deltaTime * _hipsLerpSpeed;
    }
}
