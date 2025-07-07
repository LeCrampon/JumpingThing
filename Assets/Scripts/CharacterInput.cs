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
    private CameraControls _camera;

    private bool hasStartedMoving = false;

    private void Awake()
    {
        _jumpingMovement = GetComponent<JumpingMovement>();
        _characterMovement = GetComponent<CharacterMovement>();
    }


    public void OnStartFlying(InputValue value)
    {
        _characterMovement.StartFlying();
    }

    public void OnMove(InputValue value)
    {
        _moveValue = value.Get<Vector2>();
        _characterMovement.SetMoveValue(_moveValue);

        if (!hasStartedMoving && _moveValue != Vector2.zero)
        {
            hasStartedMoving = true;
            _characterMovement.OnMoveStarted();
        }

        if(hasStartedMoving && _moveValue == Vector2.zero)
        {
            hasStartedMoving = false;
            _characterMovement.OnMoveEnded();
        }
    }

    public void OnLook(InputValue value)
    {
        //Debug.Log("ON LOOK");
        _lookValue = value.Get<Vector2>();
        _camera.SetLookValue(_lookValue);
    }

}
