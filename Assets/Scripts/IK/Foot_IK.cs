using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot_IK : MonoBehaviour
{
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
    [SerializeField]
    private float _gaitOffset;

    public bool _isFootDown = true;
    public bool _isMoving = false;

    [Header("DEBUG")]
    [SerializeField]
    private Transform _debugRaycastTransform;


    private void Awake()
    {
        _currentPosition = transform.position;
        _nextPosition = transform.position;
    }

    public void ManageFootMovement()
    {

        FindPointInFront();

        if (_isMoving)
        {
            UpdateLegMovement();
        }

    }


    private void FindNextPoint()
    {
        RaycastHit hit;
        if (Physics.Raycast(_raycastOrigin.position, _characterIK._groundChecker._groundedDirection, out hit, _raycastDistance, _characterIK._groundMask))
        {
            if (Vector3.Distance(_nextPosition, hit.point) > _characterIK._stepSize && !_isMoving)
            {
                //_stepLerp = 0;
                _nextPosition = hit.point;
                StartLegMovement(); 

            }
            _debugRaycastTransform.position = hit.point;
        }

        Debug.DrawRay(_raycastOrigin.position, _characterIK._groundChecker._groundedDirection, Color.red);
    }

    public void FindPointInFront()
    {
        //Vector3 newVector = Vector3.Cross(-_characterIK._groundChecker._groundedDirection, Vector3.forward).normalized;
        //Vector3 vecteur2 = Vector3.Cross(-_characterIK._groundChecker._groundedDirection, newVector).normalized;

        Vector3 newDirection = Vector3.ProjectOnPlane(_characterIK._groundChecker.transform.forward, -_characterIK._groundChecker._groundedDirection);

        Debug.DrawRay(_raycastOrigin.position, newDirection, Color.blue);
        //Debug.DrawRay(_raycastOrigin.position, newVector, Color.yellow);
        //Debug.DrawRay(_raycastOrigin.position, vecteur2, Color.green);

        RaycastHit hit;
        if (Physics.Raycast(_raycastOrigin.position, newDirection, out hit, .1f, _characterIK._groundMask))
        {
            if (Vector3.Distance(_nextPosition, hit.point) > _characterIK._stepSize)
            {
                _stepLerp = 0;
                _nextPosition = hit.point;
            }
            _debugRaycastTransform.position = hit.point;
        }
        else
        {
            FindNextPoint();
        }
    }

    private void MoveToNextPosition()
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
            _isFootDown = true;
            _previousPosition = _nextPosition;
        }

        transform.position = _currentPosition;
    }

    public void StartLegMovement()
    {
        _isMoving = true;
        _stepLerp = 0;
        //_nextPosition += transform.forward * _gaitOffset * .2f;
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
        ManageFootMovement();

    }
}
