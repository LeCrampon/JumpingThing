using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
    private LayerMask _collisionLayerMask;
    [SerializeField]
    private float _collisionOffset;
    [SerializeField]
    private float _mouseSensitivity;
    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Vector3 finalPosition = new Vector3(_target.position.x, 0, _target.position.z);
        transform.position = finalPosition - _target.forward * _distanceOffset;
        transform.position = new Vector3(transform.position.x, transform.position.y + _heightOffset, transform.position.z);
        //transform.rotation = Quaternion.LookRotation(finalPosition - transform.position);
    }

    void Update()
    {
        if(_mainCharacter._movementType == MovementType.JumpingMovement)
        {
            SmoothCameraMovement();
        }
        else if(_mainCharacter._movementType == MovementType.CrawlingMovement)
        {
            //RotateCameraMovement();
            FollowTarget();
        }

    }

    void FollowTarget()
    {
        transform.position = _target.position;
    }

    public void SetLookValue(Vector2 lookValue)
    {
        _lookValue = lookValue;
    }

    private void OLDRotateCameraMovement()
    {
        xRotation -= Mathf.Clamp(_lookValue.y * _mouseSensitivity * Time.deltaTime, -30f, 70f);
        yRotation += _lookValue.x * _mouseSensitivity * Time.deltaTime;

        //Vector3 avoid = ObstacleDetection(transform.position - _target.position);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.position = _target.position - transform.forward * _distanceOffset;

    }

    private void RotateCameraMovement()
    {
        //Récupérer valeurs de mouse
        yRotation -= _lookValue.y * _mouseSensitivity * Time.deltaTime;
        xRotation += _lookValue.x * _mouseSensitivity * Time.deltaTime;

        //Clamper
        xRotation = ClampAngle(xRotation, -360, 360);
        yRotation = ClampAngle(yRotation, 10, 80);

        Quaternion newRotation = Quaternion.Euler(yRotation, xRotation, 0);
        Vector3 distanceVector = new Vector3(0, 0f, -_distanceOffset);
        Vector3 newPosition = newRotation * distanceVector + _target.position;

     
        //TODO: rendre le clamp dépendant du vector.up
        

        transform.rotation = newRotation;
        transform.position = SphereCastDetection(transform.position, newPosition, .15f); ;

    }

    private float ClampAngle(float value, float min, float max)
    {
        if(value < -360f)
        {
            value += 360f;
        }

        if( value > 360f)
        {
            value -= 360f;
        }

        return Mathf.Clamp(value, min, max);
    }


    private Vector3 SphereCastDetection(Vector3 position, Vector3 targetPos, float radius)
    {
        Vector3 dir = targetPos - transform.position;
        float distance = Vector3.Distance(targetPos, transform.position) +.1f;

        Vector3 finalPosition = targetPos;
        RaycastHit hit;
        if (Physics.SphereCast(position, radius, dir , out hit, distance, _collisionLayerMask))
        {
            finalPosition = hit.point -dir * .1f;
        }

        Debug.DrawLine(position, targetPos, Color.red);
        return finalPosition;
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
