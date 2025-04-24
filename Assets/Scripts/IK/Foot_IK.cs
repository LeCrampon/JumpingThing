using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot_IK : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CharacterMovement _characterMovement;
    [SerializeField]
    private CharacterIK _characterIK;
    [SerializeField]
    private float _raycastDistance;
    [SerializeField]
    private Transform _raycastOrigin;

    private Vector3 _currentPosition;
    private Vector3 _previousPosition;
    private Vector3 _nextPosition;
    private float _stepLerp = 1;

    [Header("Bools")]
    public bool _isFootDown = true;
    public bool _isMoving = false;

    [Header("DEBUG")]
    [SerializeField]
    private Transform _debugRaycastTransform;

    private Foot_GroundCheck _footGroundCheck;

    public Foot_IK _oppositeLeg;

    private void Awake()
    {
        _currentPosition = transform.position;
        _nextPosition = transform.position;

        _footGroundCheck = GetComponent<Foot_GroundCheck>();
    }

    public void ManageFootMovement()
    {
        //if (_characterMovement._isMoving)
        //{
        //    SetNewPosition();
        //}
        //else
        //{
        //    STATIC_SetNewPosition();
        //}
        STATIC_SetNewPosition();
        UpdateLegMovement();

    }


    //new
    private void STATIC_SetNewPosition()
    {
        if (Vector3.Distance(_nextPosition, _footGroundCheck.GetDestination().position) > _characterIK._stepSize && !_isMoving)
        {
            _nextPosition = _footGroundCheck.GetDestination().position;
            StartLegMovement();
        }
    }

    private void SetNewPosition()
    {
        if ( !_isMoving)
        {
            _nextPosition = _footGroundCheck.GetDestination().position;
            StartLegMovement();
        }
    }

    public void StartLegMovement()
    {
        _isMoving = true;
        _stepLerp = 0;
        _footGroundCheck.DEBUG_UpdateDebugRayCastPosition();
    }

    public void UpdateLegMovement()
    {
        if (_stepLerp < 1)
        {
            _isFootDown = false;
            Vector3 footPosition = Vector3.Lerp(_previousPosition, _nextPosition, _stepLerp);
            footPosition.y += Mathf.Sin(_stepLerp * Mathf.PI) * _characterIK._stepHeight;
            _currentPosition = footPosition;
            _stepLerp += Time.deltaTime * _characterIK._stepSpeed;
        }
        else
        {
            _isMoving = false;
            _isFootDown = true;
            _previousPosition = _nextPosition;
        }
        transform.position = _currentPosition;
    }

    private void Update()
    {
        //ManageFootMovement();
    }
}
