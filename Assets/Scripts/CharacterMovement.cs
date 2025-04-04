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

}

public enum MovementType
{
    JumpingMovement,
    CrawlingMovement
}
