using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour
{
    private JumpingMovement _jumpingMovement;
    private CharacterMovement _characterMovement;
    [SerializeField]
    private Vector2 _moveValue;
    [SerializeField]
    private Vector2 _lookValue;
    [SerializeField]
    private FollowCamera _camera;

    private void Awake()
    {
        _jumpingMovement = GetComponent<JumpingMovement>();
        _characterMovement = GetComponent<CharacterMovement>();
    }

    private void OnJump(InputValue value)
    {
        //_jumpingMovement.Jump();
    }

    public void OnMove(InputValue value)
    {
        _moveValue = value.Get<Vector2>();
        _characterMovement.SetMoveValue(_moveValue);
    }

    public void OnLook(InputValue value)
    {
        _lookValue = value.Get<Vector2>();
        _camera.SetLookValue(_lookValue);
    }

}
