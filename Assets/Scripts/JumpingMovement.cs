using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingMovement : MonoBehaviour
{
    [Header("Movement Bools")]
    [SerializeField]
    private bool _isJumping;

    [Header("Jump Parameters")]
    [SerializeField]
    private float _jumpHeight;
    [SerializeField]
    private float _jumpLength;
    [SerializeField]
    private float _jumpDuration;
    [SerializeField]
    private AnimationCurve _jumpCurveY;
    [SerializeField]
    private float _jumpTimer;
    private Vector3 _startJumpPos;
    private Vector3 _endJumpPos;
    private Vector3 _collisionJumpPos;

    [Header("RayCast")]
    [SerializeField]
    private LayerMask _groundMask;
    [SerializeField]
    private LayerMask _wallMask;

    [Header("References")]
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private GameObject _playerModel;
    [SerializeField]
    private CharacterMovement _characterMovement;
    [SerializeField]
    private GroundChecker _groundChecker;


    [Header("DEBUG")]
    [SerializeField]
    private Transform _dStartJumpTransform;
    [SerializeField]
    private Transform _dEndJumpTransform;
    [SerializeField]
    private Transform _dTempJumpTransform;
    [SerializeField]
    private Transform _dJumpCollision;
    [SerializeField]
    private Transform[] _dTrajectory;

    //Should handle everything related to jumping movement, to use in Update
    public void HandleJumpingMovement(Vector2 moveValue)
    {
        HandleMoveJumping(moveValue);
        HandleJump();
    }

    private void ManageJumpRotation()
    {
        transform.LookAt(new Vector3(_endJumpPos.x, transform.position.y, _endJumpPos.z));
    }

    #region JumpManagement
    private void FindJumpDestination(Vector2 moveValue)
    {
        _startJumpPos = transform.position;
        Vector3 forward = (_mainCamera.transform.forward * moveValue.y + _mainCamera.transform.right * moveValue.x).normalized;

        //Vector3 tempPositition = _startJumpPos + new Vector3(_moveValue.x,0, _moveValue.y).normalized * _jumpLength;
        Vector3 tempPositition = _startJumpPos + forward * _jumpLength;
        tempPositition += Vector3.up * _jumpHeight;
        RaycastHit hit;
        float yPos = 0;
        if(Physics.Raycast(tempPositition, Vector3.down, out hit, 10f, _groundMask))
        {
            yPos = hit.point.y;
        }
        _endJumpPos = new Vector3(tempPositition.x, yPos, tempPositition.z) ;

        ManageJumpRotation();

        _dEndJumpTransform.position = _endJumpPos;
        _dStartJumpTransform.position = _startJumpPos;
        _dTempJumpTransform.position = tempPositition;
    }

    private void FindJumpDestinationWall()
    {
        _startJumpPos = transform.position;
        _endJumpPos = _collisionJumpPos;

        ManageJumpRotation();
    }

    private void SetJumpCurve()
    {
        float newHeight = _startJumpPos.y + _jumpHeight;
        Debug.Log("HEIGHT: " + _startJumpPos.y + " + " + _jumpHeight + " = " + newHeight);

        Keyframe[] keys = new Keyframe[3];
        keys[0] = new Keyframe(0f, _startJumpPos.y);
        keys[1] = new Keyframe(_jumpDuration / 2, newHeight);
        keys[2] = new Keyframe(_jumpDuration, _endJumpPos.y);

        _jumpCurveY = new AnimationCurve(keys);

        //foreach (Keyframe key in _jumpCurveY.keys)
        //{
        //    Debug.Log($"Temps : {key.time}, Valeur : {key.value}");
        //}
     
    }

    private bool IsJumpColliding(int resolution)
    {
        Vector3[] points = EvaluateJumpPositions(resolution);

        for (int i =1; i< points.Length; i++)
        {

            Vector3 direction = points[i] - points[i - 1];
            float distance = direction.magnitude;
            Debug.DrawRay(points[i - 1], direction, Color.green, 5f);
            RaycastHit hit;
            if (Physics.Raycast(points[i - 1], direction.normalized, out hit, distance, _wallMask))
            {
                //COLLIDING!
                _dJumpCollision.position = hit.point;
                Debug.Log($"COLLIDING AT : {hit.point} ");
                _collisionJumpPos = hit.point;
                return true;

            }

        }

        return false;
    }

    private Vector3[] EvaluateJumpPositions(int resolution)
    {
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float timer = i / (float)(resolution - 1);
            float jumpHeight = _jumpCurveY.Evaluate(timer * _jumpDuration);
            points[i] = Vector3.Lerp(_startJumpPos, _endJumpPos, timer) + Vector3.up * jumpHeight;
            _dTrajectory[i].position = points[i];
        }
        return points;
    }

    public void Jump(Vector2 moveValue)
    {
        if (_groundChecker.CheckGround() && !_isJumping)
        {
            FindJumpDestination(moveValue);
            SetJumpCurve();
            if (!IsJumpColliding(10))
            {
                _isJumping = true;
                _jumpTimer = 0;
            }
            else
            {
                FindJumpDestinationWall();
                SetJumpCurve();
                //Walk on Wall mode
                //SwitchToVerticalMovement();
                _isJumping = true;
                _jumpTimer = 0;
            }
        }

    }

    private void HandleJump()
    {
        if (_isJumping)
        {
            _jumpTimer += Time.deltaTime;
            float verticalMovement = _jumpCurveY.Evaluate(_jumpTimer);

            Vector2 startPos = new Vector2(_startJumpPos.x, _startJumpPos.z);
            Vector2 endPos = new Vector2(_endJumpPos.x, _endJumpPos.z);
            Vector2 horizontalMovement = Vector3.Lerp(startPos, endPos, _jumpTimer / _jumpDuration);

            if (_jumpTimer >= _jumpDuration)
            {
                _isJumping = false;
            }

            transform.position = new Vector3(horizontalMovement.x, verticalMovement, horizontalMovement.y);
        }
    }

    #endregion

   
    private void HandleMoveJumping(Vector2 moveValue)
    {
        if (_characterMovement.CheckMoving())
        {
            Jump(moveValue);
        }
    }

  


    
}

