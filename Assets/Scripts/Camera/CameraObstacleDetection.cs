using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObstacleDetection : MonoBehaviour
{
    [SerializeField]
    private CharacterMovement _characterMovement;
    [SerializeField]
    public Transform _cameraTransform;
    [SerializeField]
    private Transform _cameraTargetTransform;

    [SerializeField]
    private LayerMask _collisionLayerMask;
    [SerializeField]
    private float _obstacleDistanceOffset;
    [SerializeField]
    private float _smoothFactor;
    [SerializeField]
    private GroundChecker _groundChecker;

    [Header("Camera Distance Values ")]
    [SerializeField] 
    private float _cameraGroundDistance;
    [SerializeField]
    private float _cameraWallDistance;
    [SerializeField]
    private float _cameraUpsideDownDistance;
    [SerializeField]
    private float _cameraFlightDistance;
    [SerializeField]
    private float _cameraDistance;
    [SerializeField]
    private float _cameraTransitionLerp;
    [SerializeField]
    private float _sphereCastRadius;



    public float _currentDistance;


    [Header("DEBUG")]
    [SerializeField]
    private Transform D_RayCastHit;


    private void Awake()
    {
        _currentDistance = (_cameraTargetTransform.position - transform.position).magnitude;
    }

    private void MoveCameraToMaxDistance()
    {
        _cameraTargetTransform.localPosition = new Vector3(_cameraTargetTransform.localPosition.x, _cameraTargetTransform.localPosition.y, -_cameraDistance);
    }

    private void LateUpdate()
    {
        if(_cameraTransform == null || GameStateManager._instance._isInTransition)
        {
            return;
        }
        if (_characterMovement._movementType == MovementType.CrawlingMovement /*|| _characterMovement._movementType == MovementType.JumpingMovement*/ )
        {
            MoveCameraToMaxDistance();

            switch (_groundChecker._niveauABulles)
            {
                case NiveauABulles.Ground:
                    ManageGroundCamera();
                    break;
                case NiveauABulles.Wall:
                    ManageWallCamera();
                    break;
                case NiveauABulles.UpsideDown:
                    ManageUpsideDownCamera();
                    break;
                default:
                    ManageGroundCamera();
                    break;
            }
        }
        else if(_characterMovement._movementType == MovementType.JumpingMovement)
        {
            MoveCameraToMaxDistance();
            ManageGroundCamera();
        }
        else if (_characterMovement._movementType == MovementType.FlyingMovement)
        {
            MoveCameraToMaxDistance();
            ManageFlyingCamera();
        }
    }

    private float GetCameraDistance(Vector3 castDirection)
    {
        float distance = castDirection.magnitude + _obstacleDistanceOffset;

        //Debug.DrawRay(transform.position, castDirection, Color.red, 2f);
        if (Physics.SphereCast(new Ray(transform.position, castDirection), _sphereCastRadius, out RaycastHit hit, distance, _collisionLayerMask, QueryTriggerInteraction.Ignore))
        {
            if(D_RayCastHit != null)
                D_RayCastHit.position = hit.point;
            return Mathf.Max(0f, hit.distance - _obstacleDistanceOffset);
  
        }
        

        return castDirection.magnitude;
    }

    private void ManageFlyingCamera()
    {
        _cameraDistance = Mathf.Lerp(_cameraDistance, _cameraFlightDistance, _cameraTransitionLerp* Time.deltaTime);


        Vector3 castDirection = _cameraTargetTransform.position - transform.position;
        float distance = GetCameraDistance(castDirection);
        //Debug.Log("CAMERA DISTANCE " + distance);
        _currentDistance = Mathf.Lerp(_currentDistance, distance, Time.deltaTime * _smoothFactor);
        _cameraTransform.position = transform.position + castDirection.normalized * _currentDistance;
    }

    private void ManageGroundCamera()
    {
        _cameraDistance = Mathf.Lerp(_cameraDistance, _cameraGroundDistance, _cameraTransitionLerp * Time.deltaTime);


        Vector3 castDirection = _cameraTargetTransform.position - transform.position;
        float distance = GetCameraDistance(castDirection);
        //Debug.Log("CAMERA DISTANCE " + distance);
        _currentDistance = Mathf.Lerp(_currentDistance, distance, Time.deltaTime * _smoothFactor);
        _cameraTransform.position = transform.position + castDirection.normalized * _currentDistance;
    }

    private void ManageWallCamera()
    {
        _cameraDistance = Mathf.Lerp(_cameraDistance, _cameraWallDistance, _cameraTransitionLerp * Time.deltaTime);


        Vector3 castDirection = _cameraTargetTransform.position - transform.position;
        float distance = GetCameraDistance(castDirection);
        _currentDistance = Mathf.Lerp(_currentDistance, distance, Time.deltaTime * _smoothFactor);

        _cameraTransform.position = transform.position + castDirection.normalized * _currentDistance;
    }

    private void ManageUpsideDownCamera()
    {
        _cameraDistance = Mathf.Lerp(_cameraDistance, _cameraUpsideDownDistance, _cameraTransitionLerp * Time.deltaTime);
    }
}

