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
    public CrawlingMovement _crawlingMovement;
    [SerializeField]
    public JumpingMovement _jumpingMovement;
    [SerializeField]
    public FlyingMovement _flyingMovement;
    public Camera _mainCamera;
    [SerializeField]
    public Transform _cameraTarget;
    [SerializeField]
    public CameraControls _cameraControls;

    [Header("Movement bools")]
    public MovementType _movementType = MovementType.JumpingMovement;
    [SerializeField]
    public bool _isMoving;

    [Header("Audio")]
    [SerializeField]
    private FootStepAudio _footStepAudio;
    [SerializeField]
    private FlyingAudio _flyingAudio;

    private Vector2 _moveValue;

    private float _switchTimer = 0f;
    [SerializeField]
    private float _switchTime = .2f;
    private bool _justSwitched = false;

    public bool _isPoisoned = false;


    private void Update()
    {
        //if (GameStateManager._instance._isInMenu)
        //{
        //    return;
        //}
        if (_movementType == MovementType.CrawlingMovement)
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


        //poisoning
        if (GameStateManager._instance.GetCurrentCharacter() == this)
        {
            //Debug.Log("GetCurrentCharacter " + this.gameObject.name);
            if (_isPoisoned)
            {
                GameStateManager._instance.GetPostProcess().StartPoisoning();
                GameStateManager._instance.SwitchMusicToPoisoned(this);
            }
            else
            {
                GameStateManager._instance.GetPostProcess().StopPoisoning();
                GameStateManager._instance.SwitchMusicFromPoisoned(this);
            }
        }

        WaitForSwitch();

    }

    public void StartFlying()
    {
        if (!_isFlyingCreature)
            return;
        if(_movementType == MovementType.CrawlingMovement)
        {
            SwitchToFlyingMovement();
        }
        //else
        //{
        //    SwitchToCrawlingMovement();
        //}
    }

    public void JumpOffCliff() {
        if (!_isJumpingCreature)
            return;
        if(_movementType == MovementType.CrawlingMovement)
        {
            _jumpingMovement.JumpOff();
            SwitchToJumpingovement();
        }
    }

    private void WaitForSwitch()
    {

        if (!_justSwitched)
        {
            return;
        }
        if(_switchTimer >= _switchTime)
        {
            _switchTimer = 0;
            _justSwitched = false;
        }
        _switchTimer += Time.deltaTime;

    }

    public void OnMoveStarted()
    {
        if(_movementType == MovementType.CrawlingMovement && GameStateManager._instance.GetCurrentCharacter() == this)
        {
            _footStepAudio.StartFootStepAudio();
        }

    }

    public void OnMoveEnded()
    {
        if (_movementType == MovementType.CrawlingMovement && GameStateManager._instance.GetCurrentCharacter() == this)
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
            if(_movementType == MovementType.FlyingMovement)
            {
                _flyingMovement._animator.SetBool("isFlying", false);
                _flyingAudio.StopFlyingAudio();
            }
            _movementType = MovementType.CrawlingMovement;

            if (_isMoving)
            {
                _footStepAudio.StartFootStepAudio();
            }
            Debug.Log("SWITCHING TO CRAWLING");
            _justSwitched = true;
        }

    }

    public void SwitchToFlyingMovement()
    {
        if (!_justSwitched)
        {
            _movementType = MovementType.FlyingMovement;
            _flyingMovement._animator.SetBool("isFlying", true);
            _flyingMovement._isTakingOff = true;
            Debug.Log("SWITCHING TO Flying");
            _justSwitched = true;
            _footStepAudio.StopFootStepAudio();
            _flyingAudio.StartFlyingAudio();
        }

    }

    public void SwitchToJumpingovement()
    {
        if (!_justSwitched)
        {
            _movementType = MovementType.JumpingMovement;
            Debug.Log("SWITCHING TO JUMPING");
            _justSwitched = true;
            _footStepAudio.StopFootStepAudio();
        }
    }

   
}

public enum MovementType
{
    JumpingMovement,
    CrawlingMovement,
    FlyingMovement
}
