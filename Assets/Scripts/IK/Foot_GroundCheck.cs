using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot_GroundCheck : MonoBehaviour
{
    [SerializeField]
    private LayerMask _groundMask;
    [Header("Raycast References")]
    [SerializeField]
    private Transform _raycastTransformDown;
    [SerializeField]
    private Transform _raycastTransformBack;

    [Header("Raycast Distances")]
    [SerializeField]
    private float _rayCastDistanceDown;
    [SerializeField]
    private float _rayCastDistanceFront;
    [SerializeField]
    private float _rayCastDistanceBack;

    private IK_TargetDestination _destination = new IK_TargetDestination();

    [Header("Directions")]
    [SerializeField]
    private LegFrontBack _legDirection;
    [SerializeField]
    private LegLeftRight _legSide;



    [Header("DEBUG")]
    [SerializeField]
    private Transform _DEBUG_DESTINATION;



    private void Update()
    {
        SetDestination();

    }

    public void DEBUG_UpdateDebugRayCastPosition()
    {
        _DEBUG_DESTINATION.position = _destination.position;
    }

    public IK_TargetDestination GetDestination()
    {
        return _destination;
    }

    public void SetDestination()
    {
        //////DEBUG
        Debug.DrawLine(_raycastTransformDown.position, _raycastTransformDown.position + _raycastTransformDown.forward * _rayCastDistanceFront, Color.green, 1f);
        Debug.DrawLine(_raycastTransformDown.position, _raycastTransformDown.position - _raycastTransformDown.up * _rayCastDistanceDown, Color.red, 1f);
        Debug.DrawLine(_raycastTransformBack.position, _raycastTransformBack.position - _raycastTransformBack.forward * _rayCastDistanceBack, Color.blue, 1f);
        //////DEBUG

        RaycastHit hit;
        //CHECK DEVANT
        if (Physics.Raycast(_raycastTransformDown.position, _raycastTransformDown.forward, out hit, _rayCastDistanceFront, _groundMask))
        {
            _destination.SetPositionAndNormal(hit.point, hit.normal);
            return;
        }
        //CheckDown
        if (Physics.Raycast(_raycastTransformDown.position, -_raycastTransformDown.up, out hit, _rayCastDistanceDown, _groundMask))
        {
            _destination.SetPositionAndNormal(hit.point, hit.normal);
            return;
        }

        //Check Backward
        if (Physics.Raycast(_raycastTransformBack.position, -_raycastTransformBack.forward, out hit, _rayCastDistanceBack, _groundMask))
        {
            _destination.SetPositionAndNormal(hit.point, hit.normal);
            return;
        }

        int frontModifier = _legDirection == LegFrontBack.Front ? 1 : -1;
        Vector3 insideDirection = _legSide == LegLeftRight.Left ? _raycastTransformBack.right : -_raycastTransformBack.right;
        insideDirection = (insideDirection * frontModifier).normalized ;

        //Check Inside
        if (Physics.Raycast(_raycastTransformBack.position, insideDirection, out hit, _rayCastDistanceBack, _groundMask))
        {
            _destination.SetPositionAndNormal(hit.point, hit.normal);
            return;
        }
     
    }
}



public enum LegFrontBack
{
    Front,
    Middle,
    Back
}

public enum LegLeftRight
{
    Left,
    Right
}

public class IK_TargetDestination
{
    public Vector3 position;
    public Vector3 normal;

    public IK_TargetDestination()
    {
        position = Vector3.zero;
        normal = Vector3.up;
    }

    public void SetPositionAndNormal(Vector3 newPosition, Vector3 newNormal) 
    {
        position = newPosition;
        normal = newNormal;
    }
}
