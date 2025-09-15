using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInput : MonoBehaviour
{
    private PlayerInput _playerInput;
    private JumpingMovement _jumpingMovement;
    private CharacterMovement _characterMovement;
    [SerializeField]
    private Vector2 _moveValue;
    [SerializeField]
    private Vector2 _lookValue;
    [SerializeField]
    private CameraControls _cameraControls;

    private bool hasStartedMoving = false;
    private bool _inRadialMenu = false;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        //_jumpingMovement = GetComponent<JumpingMovement>();
        //_characterMovement = GetComponent<CharacterMovement>();
    }

    public void SwitchCharacterMovement(CharacterMovement characterMovement)
    {
        //_playerInput = GetComponent<PlayerInput>();
        _jumpingMovement = characterMovement._jumpingMovement;
        _characterMovement = characterMovement;
        _cameraControls = characterMovement._cameraControls;
    }

    private void Update()
    {
        //if (GameStateManager._instance._isInMenu)
        //{
        //    _playerInput.SwitchCurrentActionMap("UI");
        //}
        //else
        //{
        //    _playerInput.SwitchCurrentActionMap("Player");
        //    Debug.Log("Switching Actions");
        //}
    }

    public void SwitchActionMap(string actionMap)
    {
        _playerInput.SwitchCurrentActionMap(actionMap);
    }

    public void OnStartFlying(InputValue value)
    {
        if(_characterMovement != null)
            _characterMovement.StartFlying();
        Debug.Log("StartFlying");
    }

    public void OnMove(InputValue value)
    {
        if (_characterMovement != null)
        {
            _moveValue = value.Get<Vector2>();
            _characterMovement.SetMoveValue(_moveValue);

            if (!hasStartedMoving && _moveValue != Vector2.zero)
            {
                hasStartedMoving = true;
                _characterMovement.OnMoveStarted();
            }

            if (hasStartedMoving && _moveValue == Vector2.zero)
            {
                hasStartedMoving = false;
                _characterMovement.OnMoveEnded();
            }

            Debug.Log("OnMove : " + _moveValue);
        }
            
    }

    private void ResetAllValues()
    {
        _moveValue = Vector2.zero;
        _lookValue = Vector2.zero;
        _characterMovement.SetMoveValue(_moveValue);
        _cameraControls.SetLookValue(_lookValue);
    }

    public void OnSwitchCharacter(InputValue value)
    {
        Debug.Log("Switching chara");
        //GameStateManager._instance._cameraTransitionHelper.GoToNextCharacter();

        if (value.isPressed)
        {
            GameStateManager._instance.OpenRadialMenu();
        }
        else
        {
            GameStateManager._instance.CloseRadialMenu();
        }
    }

    public void OnLook(InputValue value)
    {
        if(_cameraControls != null)
        {
            //Debug.Log("ON LOOK");
            _lookValue = value.Get<Vector2>();
            //Debug.Log("Character Input lookValue " + _lookValue);
            _cameraControls.SetLookValue(_lookValue);
        }
       
    }

    public void OnPause(InputValue value)
    {
        GameStateManager._instance.PauseGame();
        ResetAllValues();
    }

}
