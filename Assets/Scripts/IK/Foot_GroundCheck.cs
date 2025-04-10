using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foot_GroundCheck : MonoBehaviour
{
    [SerializeField]
    private Transform _raycastTransformDown;
    [SerializeField]
    private Transform _raycastTransformBack;

    [SerializeField]
    private float _rayCastDistanceDown;
    [SerializeField]
    private float _rayCastDistanceFront;
    [SerializeField]
    private float _rayCastDistanceBack;

    [SerializeField]
    private IK_TargetDestination _destination;

    [SerializeField]
    private LegFrontBack _legDirection;
    [SerializeField]
    private LegFrontBack _normal;

    [SerializeField]
    private LegLeftRight _legSide;

    [SerializeField]
    private LayerMask _groundMask;

    

    public IK_TargetDestination GetDestination()
    {
        IK_TargetDestination destination = new IK_TargetDestination();


        RaycastHit hit;
        //CHECK DEVANT
        if (Physics.Raycast(_raycastTransformDown.position, _raycastTransformDown.forward, out hit, _rayCastDistanceFront, _groundMask))
        {
            destination.SetPositionAndNormal(hit.point, hit.normal);
        }
        else
        {
            //CheckDown
            if (Physics.Raycast(_raycastTransformDown.position, -_raycastTransformDown.up, out hit, _rayCastDistanceDown, _groundMask))
            {
                destination.SetPositionAndNormal(hit.point, hit.normal);
            }
            else
            {
                //Check Backward
                if (Physics.Raycast(_raycastTransformBack.position, -_raycastTransformBack.forward, out hit, _rayCastDistanceBack, _groundMask))
                {
                    destination.SetPositionAndNormal(hit.point, hit.normal);
                }
            }
        }

        Debug.DrawLine(_raycastTransformDown.position, _raycastTransformDown.position + _raycastTransformDown.forward * _rayCastDistanceFront, Color.green, 1f);
        Debug.DrawLine(_raycastTransformDown.position, _raycastTransformDown.position + -_raycastTransformDown.up * _rayCastDistanceDown, Color.green, 1f);
        Debug.DrawLine(_raycastTransformBack.position, _raycastTransformBack.position - -_raycastTransformBack.forward * _rayCastDistanceBack, Color.green, 1f);

        _destination = destination;
        return destination;
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
