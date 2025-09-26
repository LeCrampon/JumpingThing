using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class FlyingMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CharacterMovement _characterMovement;
    [SerializeField]
    private GroundChecker _groundChecker;
    [SerializeField]
    public Animator _animator;
    [SerializeField]
    private CameraControls _cameraControls;
    [SerializeField]
    public Transform _flyingFeetTarget;
    [SerializeField]
    private Foot_GroundCheck[] _feetGroundChecks;

    [Header("Detection")]
    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private float _raycastDistance =1f;
    [SerializeField]
    private Transform _raycastStart;

    [Header("Movement Values")]
    [SerializeField]
    private float _forwardSpeed = 5f;
    [SerializeField]
    private float _takeOffSpeed = 2f;
    [SerializeField]
    private float _rotationSpeed = 1.5f;

    private float _nextHitDistance;

    [SerializeField]
    private float _hitDistanceToLand = .2f;

    public bool _isTakingOff;
    [SerializeField]
    private float _takeOffDuration;
    private float _takeOffTimer = 0;

    private string _lastDirection;
    private Vector3 _lastUp;

    private Vector3 _lastForward;

    private void Awake()
    {
        _lastForward = transform.forward;
    }

    public void HandleFlyingMovement(Vector2 moveValue)
    {
        //foreach(Foot_GroundCheck footCheck in _feetGroundChecks)
        //{
        //    footCheck.SetDestination(_flyingFeetTarget.position, _flyingFeetTarget.up);
        //}

        HandleRotation();

        //GESTION DU DECOLLAGE
        if (_isTakingOff && _takeOffTimer < _takeOffDuration)
        {
            if(_takeOffTimer == 0)
            {
                _lastUp = transform.up;
            }
            FlyUp();
            return;
        }
        _isTakingOff = false;
        _takeOffTimer = 0;

        Debug.Log("_nextHitDistance = " + _nextHitDistance);
        //Debug.Log("nextHitDistance " + _nextHitDistance);
        if(_nextHitDistance <= _hitDistanceToLand)
        {
            foreach(Foot_GroundCheck foot in GetComponent<CrawlingMovement>()._feetGroundChecks)
            {
                foot.SetDestinationByRaycasts();
            }
            
            Debug.Log("LAST DIRECTION " + _lastDirection);
            _characterMovement.SwitchToCrawlingMovement();
        }

        Vector3 newForward = _characterMovement._mainCamera.transform.forward;
        Vector3 newRight = _characterMovement._mainCamera.transform.right;

        Vector3 movementOffset = (newForward * moveValue.y + newRight * moveValue.x).normalized;

        transform.position = Vector3.Lerp(transform.position, transform.position + movementOffset, Time.deltaTime * _forwardSpeed);

        

    }

    private void FlyUp()
    {
        Vector3 takeOffOffset = _lastUp * 2;
        transform.position = Vector3.Lerp(transform.position, transform.position + takeOffOffset, Time.deltaTime * _takeOffSpeed);
        _takeOffTimer += Time.deltaTime;
    }

    private void HandleRotation()
    {
        //Detecter surface la plus proche
        Vector3 surfaceNormal = DetectSurfaces();
        Vector3 surfaceForward = Vector3.ProjectOnPlane(_characterMovement._mainCamera.transform.forward, surfaceNormal).normalized;
        Vector3 surfaceRight = Vector3.Cross(surfaceNormal, surfaceForward);

        //Debug.DrawRay(transform.position, surfaceForward, Color.blue, .1f);
        //Debug.DrawRay(transform.position, surfaceNormal, Color.blue, .1f);
        //Debug.DrawRay(transform.position, surfaceRight, Color.blue, .1f);

        //Rotation de la surface
        Quaternion surfaceRotation = Quaternion.LookRotation(surfaceForward, surfaceNormal);

        Vector3 camForward = _characterMovement._mainCamera.transform.forward;
        Vector3 camRight = _characterMovement._mainCamera.transform.right;
        Vector3 camUp = Vector3.Cross(camForward, camRight);

        //Debug.DrawRay(transform.position, camForward, Color.red, .1f);
        //Debug.DrawRay(transform.position, camRight, Color.red, .1f);
        //Debug.DrawRay(transform.position, camUp, Color.red, .1f);

        Quaternion camRotation = Quaternion.LookRotation(camForward, camUp);

        //CALCUL GAUCHE DROITE
        //Vector3 planeNormal = Vector3.Cross(_lastForward, Vector3.up);
        //float side = Vector3.Dot(planeNormal, transform.forward);
        Vector2 lookValue = _cameraControls.GetLookValue();
        Quaternion sideRot = Quaternion.Euler(0, 0, Mathf.Clamp(- lookValue.x * 2, -45, 45));

        //CALCUL GAUCHE DROITE

        Quaternion newCamRotation = Quaternion.Slerp(transform.rotation, camRotation, Time.deltaTime * _rotationSpeed);
        Quaternion finalCamRotation = Quaternion.Slerp(newCamRotation, newCamRotation * sideRot, Time.deltaTime * _rotationSpeed /2);

        //Debug.Log("HIT DISTANCE" + _nextHitDistance );
        //Debug.Log(" LERPVALUE" + (_raycastDistance - _nextHitDistance) / _raycastDistance);
        transform.rotation = Quaternion.Slerp(finalCamRotation, surfaceRotation, (_raycastDistance - _nextHitDistance) / _raycastDistance);
        Debug.Log("surfaceRotation " + surfaceRotation);


        //Debug.DrawRay(transform.position, transform.forward, Color.blue, .1f);
        //Debug.DrawRay(transform.position, transform.up, Color.green, .1f);
        //Debug.DrawRay(transform.position, transform.right, Color.red, .1f);
    }

    private Vector3 DetectSurfaces()
    {
        _nextHitDistance = float.MaxValue;
        float smallestDistance = float.MaxValue;

        List<float> hitDistances = new List<float>();
        List<Vector3> hitNormals = new List<Vector3>();

        Vector3 hitNormal = Vector3.up;

        string lastSurfaceName = "";

        //front
        RaycastHit hit;
        if (Physics.Raycast(_raycastStart.transform.position, _characterMovement._mainCamera.transform.forward, out hit, _raycastDistance, _groundLayer))
        {
            hitDistances.Add(hit.distance);
            hitNormals.Add(hit.normal);
            if(hit.distance < smallestDistance)
            {
                hitNormal = hit.normal;
                _nextHitDistance = hit.distance;

                //Debug.DrawLine(_raycastStart.transform.position, hit.point, Color.red, 1f);
                Debug.DrawRay(_raycastStart.transform.position, _characterMovement._mainCamera.transform.forward, Color.blue, 1f);
                _lastDirection = "FORWARD";
                Debug.Log("DIRECTION " + _lastDirection);
                smallestDistance = hit.distance;
                lastSurfaceName = hit.collider.name;
            }

        }


        //left
        if (Physics.Raycast(_raycastStart.transform.position, -_characterMovement._mainCamera.transform.right, out hit, _raycastDistance, _groundLayer))
        {
            hitDistances.Add(hit.distance);
            hitNormals.Add(hit.normal);
            if (hit.distance < smallestDistance)
            {
                hitNormal = hit.normal;
                _nextHitDistance = hit.distance;
                //Debug.DrawLine(_raycastStart.transform.position, hit.point, Color.blue, 1f);
                Debug.DrawRay(_raycastStart.transform.position, -_characterMovement._mainCamera.transform.right, Color.blue, 1f);
                _lastDirection = "LEFT";
                Debug.Log("DIRECTION " + _lastDirection);
                smallestDistance = hit.distance;
                lastSurfaceName = hit.collider.name;
            }
        }

        //right
        if (Physics.Raycast(_raycastStart.transform.position, _characterMovement._mainCamera.transform.right, out hit, _raycastDistance, _groundLayer))
        {
            hitDistances.Add(hit.distance);
            hitNormals.Add(hit.normal);
            if (hit.distance < smallestDistance)
            {
                hitNormal = hit.normal;
                _nextHitDistance = hit.distance;
                //Debug.DrawLine(_raycastStart.transform.position, hit.point, Color.blue, 1f);
                Debug.DrawRay(_raycastStart.transform.position, _characterMovement._mainCamera.transform.right, Color.blue, 1f);
                _lastDirection = " RIGHT";
                Debug.Log("DIRECTION " + _lastDirection);
                smallestDistance = hit.distance;
                lastSurfaceName = hit.collider.name;
            }
        }

        //down
        if (Physics.Raycast(_raycastStart.transform.position, -_characterMovement._mainCamera.transform.up, out hit, _raycastDistance, _groundLayer))
        {
            hitDistances.Add(hit.distance);
            hitNormals.Add(hit.normal);
            if (hit.distance < smallestDistance)
            {
                hitNormal = hit.normal;
                _nextHitDistance = hit.distance;
                //Debug.DrawLine(_raycastStart.transform.position, hit.point, Color.green, 1f);
                Debug.DrawRay(_raycastStart.transform.position, -_characterMovement._mainCamera.transform.up, Color.blue, 1f);
                _lastDirection = "DOWN";
                Debug.Log("DIRECTION " + _lastDirection);
                smallestDistance = hit.distance;
                lastSurfaceName = hit.collider.name;
            }
        }

        //Up
        if (Physics.Raycast(_raycastStart.transform.position, _characterMovement._mainCamera.transform.up, out hit, _raycastDistance, _groundLayer))
        {
            hitDistances.Add(hit.distance);
            hitNormals.Add(hit.normal);
            if (hit.distance < smallestDistance)
            {
                hitNormal = hit.normal;
                _nextHitDistance = hit.distance;
                //Debug.DrawLine(_raycastStart.transform.position, hit.point, Color.green, 1f);
                Debug.DrawRay(_raycastStart.transform.position, _characterMovement._mainCamera.transform.up, Color.blue, 1f);
                _lastDirection = "UP";
                Debug.Log("DIRECTION " + _lastDirection);
                smallestDistance = hit.distance;
                lastSurfaceName = hit.collider.name;
            }
        }

        //back
        if (Physics.Raycast(_raycastStart.transform.position, -_characterMovement._mainCamera.transform.forward, out hit, _raycastDistance, _groundLayer))
        {
            hitDistances.Add(hit.distance);
            hitNormals.Add(hit.normal);
            if (hit.distance < smallestDistance)
            {
                hitNormal = hit.normal;
                _nextHitDistance = hit.distance;
                //Debug.DrawLine(_raycastStart.transform.position, hit.point, Color.green, 1f);
                Debug.DrawRay(_raycastStart.transform.position, -_characterMovement._mainCamera.transform.forward, Color.blue, 1f);
                _lastDirection = "BACK";
                Debug.Log("DIRECTION " + _lastDirection);
                smallestDistance = hit.distance;
                lastSurfaceName = hit.collider.name;
            }
        }

        //for(int i =0; i< hitDistances.Count; i++ )
        //{
        //    if(hitDistances[i] < smallestDistance)
        //    {
        //        smallestDistance = hitDistances[i];
        //        hitNormal = hitNormals[i];
        //    }
        //}

        //Debug.Log("HIT NORMAL = " + hitNormal); 
        Debug.Log("LAST SURFACE NAME = " + lastSurfaceName);
        return hitNormal;
    }
}
