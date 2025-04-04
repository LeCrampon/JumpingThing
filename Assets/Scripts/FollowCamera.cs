using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public float smoothPosFactor = 1f;
    public float smoothRotFactor = 1.5f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField]
    private float _heightOffset;
    [SerializeField]
    private float _distanceOffset;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private CharacterMovement _mainCharacter;

    [Header("Rotation camera")]
    [SerializeField]
    private Vector2 _lookValue;
    [SerializeField]
    private float _mouseSensitivity;
    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Vector3 finalPosition = new Vector3(_target.position.x, 0, _target.position.z);
        transform.position = finalPosition - _target.forward * _distanceOffset;
        transform.position = new Vector3(transform.position.x, transform.position.y + _heightOffset, transform.position.z);
        transform.rotation = Quaternion.LookRotation(finalPosition - transform.position);
    }

    void Update()
    {
        if(_mainCharacter._movementType == MovementType.JumpingMovement)
        {
            SmoothCameraMovement();
        }
        else if(_mainCharacter._movementType == MovementType.CrawlingMovement)
        {
            RotateCameraMovement();
        }

    }

    public void SetLookValue(Vector2 lookValue)
    {
        _lookValue = lookValue;
    }

    private void RotateCameraMovement()
    {
        xRotation -= Mathf.Clamp(_lookValue.y * _mouseSensitivity * Time.deltaTime,-30f, 70f);
        yRotation += _lookValue.x * _mouseSensitivity * Time.deltaTime;

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.position  = _target.position - transform.forward * _distanceOffset;
       
    }
    private void SmoothCameraMovement()
    {
        //Vector3 finalPosition = new Vector3(_target.position.x, 0, _target.position.z);
        Vector3 finalPosition = new Vector3(_target.position.x, 0, _target.position.z);
        Vector3 newPos = finalPosition - _target.forward * _distanceOffset;
        newPos = new Vector3(newPos.x, newPos.y + _heightOffset, newPos.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothPosFactor);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(finalPosition - transform.position), smoothRotFactor * Time.fixedDeltaTime);
    }
}
