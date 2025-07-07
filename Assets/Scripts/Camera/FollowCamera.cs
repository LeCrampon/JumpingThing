using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FollowCamera : MonoBehaviour
{
    public float smoothPosFactor = 1f;
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
    private float _pivotOffset;

    private void Start()
    {
        Vector3 finalPosition = new Vector3(_target.position.x, 0, _target.position.z);
        transform.position = finalPosition - _target.forward * _distanceOffset;
        transform.position = new Vector3(transform.position.x, transform.position.y + _heightOffset, transform.position.z);
        _pivotOffset = _target.transform.position.y - _mainCharacter.transform.position.y;

    }

    void Update()
    {
        if(_mainCharacter._movementType == MovementType.JumpingMovement)
        {
            JumpingCameraFollow();
        }
        else if(_mainCharacter._movementType == MovementType.CrawlingMovement)
        {
            CrawlingCameraFollow();
        }
        else if(_mainCharacter._movementType == MovementType.FlyingMovement)
        {
            CrawlingCameraFollow();
        }

    }

    void CrawlingCameraFollow()
    {
        transform.position = _target.position;
    }

    public void SetLookValue(Vector2 lookValue)
    {
        _lookValue = lookValue;
    }

    private void JumpingCameraFollow()
    {
        //Get To FinalPosition
        Vector3 startJump = _mainCharacter._jumpingMovement.GetStartJumpPos();
        Vector3 endJump = _mainCharacter._jumpingMovement.GetEndJumpPos();
        float jumpTimer = _mainCharacter._jumpingMovement.GetJumpTimer();
        float jumpDuration = _mainCharacter._jumpingMovement.GetCurrentJumpDuration();

        float yPos = Mathf.Lerp(startJump.y, endJump.y, Mathf.Clamp(jumpTimer/jumpDuration, 0 , 1));
        Vector3 finalPosition = new Vector3(_target.position.x, _pivotOffset + yPos, _target.position.z);
        transform.position = finalPosition;
     
    }

}
