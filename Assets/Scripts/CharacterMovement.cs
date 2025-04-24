using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CrawlingMovement _crawlingMovement;
    [SerializeField]
    private JumpingMovement _jumpingMovement;
    public Camera _mainCamera;

    [Header("Movement bools")]
    public MovementType _movementType = MovementType.JumpingMovement;
    [SerializeField]
    public bool _isMoving;


    private Vector2 _moveValue;


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

        DEBUG_ChangeTimeScale();
    }


    public bool CheckMoving()
    {
        return _isMoving = (_moveValue != Vector2.zero);
    }

    public void SetMoveValue(Vector2 move)
    {
        _moveValue = move;
    }

    public void SwitchToCrarwlingMovement()
    {
        _movementType = MovementType.CrawlingMovement;
    }

    public void SwitchToJumpingovement()
    {
        _movementType = MovementType.JumpingMovement;
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
    CrawlingMovement
}
