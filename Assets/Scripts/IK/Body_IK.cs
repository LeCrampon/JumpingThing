using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body_IK : MonoBehaviour
{
    [SerializeField]
    private Transform _headTransform;
    [SerializeField]
    private float _stepLerp = 0;
    [SerializeField]
    private float _lerpSpeed;
    [SerializeField]
    private float _maxRotation;

    public void BobHead()
    {
        if (_stepLerp >= 1)
        {
            _stepLerp = 0;
        }
        Quaternion newRotation = Quaternion.Euler(Mathf.Sin(_stepLerp * Mathf.PI) * _maxRotation, transform.localRotation.y, transform.localRotation.z );
        _stepLerp += Time.deltaTime * _lerpSpeed;


        _headTransform.localRotation = newRotation;
    }

    public void RotateHead(Vector3 cameraForward, Vector3 up)
    {
        //Vector3 camForwardProj = Vector3.ProjectOnPlane(cameraForward, up).normalized;
        //Vector3 calculatedRight = Vector3.Cross(up, cameraForward).normalized;
        Vector3 target = _headTransform.position + cameraForward * 10f;
        transform.LookAt(target);
    }

}
