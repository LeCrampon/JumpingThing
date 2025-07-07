using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class CharacterMovement : MonoBehaviour
{
    [Header("Creature Type")]
    public bool _isJumpingCreature;
    public bool _isFlyingCreature;

    [Header("References")]
    [SerializeField]
    private CrawlingMovement _crawlingMovement;
    [SerializeField]
    public JumpingMovement _jumpingMovement;
    [SerializeField]
    private FlyingMovement _flyingMovement;
    public Camera _mainCamera;

    [Header("Movement bools")]
    public MovementType _movementType = MovementType.JumpingMovement;
    [SerializeField]
    public bool _isMoving;

    [Header("Audio")]
    [SerializeField]
    private FootStepAudio _footStepAudio;

    private Vector2 _moveValue;

    private float _switchTimer = 0f;
    [SerializeField]
    private float _switchTime = .2f;
    private bool _justSwitched = false;


    private void Update()
    {
        if(_movementType == MovementType.CrawlingMovement)
        {
            _crawlingMovement.HandleCrawlingMovement(_moveValue);
        }
        else if(_movementType == MovementType.JumpingMovement)
        {
            _jumpingMovement.HandleJumpingMovement(_moveValue);
        }
        else if(_movementType == MovementType.FlyingMovement)
        {
            _flyingMovement.HandleFlyingMovement(_moveValue);
        }

            WaitForSwitch();

        DEBUG_ChangeTimeScale();
    }

    public void StartFlying()
    {
        if (!_isFlyingCreature)
            return;
        if(_movementType == MovementType.CrawlingMovement)
        {
            _movementType = MovementType.FlyingMovement;
        }
        else
        {
            _movementType = MovementType.CrawlingMovement;
        }
    }

    private void WaitForSwitch()
    {
        if (_justSwitched )
        {
            if(_switchTimer >= _switchTime)
            {
                _switchTimer = 0;
                _justSwitched = false;
            }
            _switchTimer += Time.deltaTime;

        }
    }

    public void OnMoveStarted()
    {
        if(_movementType == MovementType.CrawlingMovement)
        {
            _footStepAudio.StartFootStepAudio();
        }

    }

    public void OnMoveEnded()
    {
        if (_movementType == MovementType.CrawlingMovement)
        {
            _footStepAudio.StopFootStepAudio();
        }
    }

    public bool CheckMoving()
    {
        return _isMoving = (_moveValue != Vector2.zero);
    }

    public void SetMoveValue(Vector2 move)
    {
        _moveValue = move;
    }

    public void SwitchToCrawlingMovement()
    {
        if (!_justSwitched)
        {
            _movementType = MovementType.CrawlingMovement;
            Debug.Log("SWITCHING TO CRAWLING");
            _justSwitched = true;
        }

    }

    public void SwitchToJumpingovement()
    {
        if (!_justSwitched)
        {
            _movementType = MovementType.JumpingMovement;
            Debug.Log("SWITCHING TO JUMPING");
            _justSwitched = true;
        }
    }

    private void DEBUG_ChangeTimeScale()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale += .1f;
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Time.timeScale -= .1f;
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Time.timeScale = 0;
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Time.timeScale = 1;
        }
    }
}

public enum MovementType
{
    JumpingMovement,
    CrawlingMovement,
    FlyingMovement
}
