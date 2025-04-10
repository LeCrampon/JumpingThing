using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField]
    private LayerMask _groundMask;
    [Header("Reference Objects")]
    [SerializeField]
    private CrawlingMovement _crawlingMovement;

    [Header("Reference Transforms")]
    [SerializeField]
    private Transform _groundedRaycastTransform;
    [SerializeField]
    private Transform _forwardGroundedRaycastTransform;
    [SerializeField]
    private Transform _belowGroundedRaycastTransform;


    [Header("Important Values")]
    [SerializeField]
    private bool _isGrounded;
    [SerializeField]
    public Vector3 _groundedDirection = Vector3.down;
    [SerializeField]
    public Vector3 _previousGroundedDirection = Vector3.down;
    [SerializeField]
    public Vector3 _nextGroundedDirection = Vector3.down;

    [Header("TESTINg SOMETHING")]
    [SerializeField]
    private Transform[] _legsTransformRaycasts;

    public void SetGroundedDirection()
    {
        Vector3 direction = Vector3.zero;
        foreach(Transform t in _legsTransformRaycasts)
        {
            Debug.Log("NAME : " + t.name);
            Debug.DrawRay(t.position, t.up, Color.blue, 15f);

            RaycastHit hit;
            if (Physics.Raycast(t.position, t.up, out hit, 1f, _groundMask))
            {
                direction += -hit.normal;
                Debug.Log("DIRECTION : " + direction.x + " " + direction.y + " " + direction.z);
            }


            Debug.DrawLine(t.position, t.position + _groundedDirection * 10, Color.red);
        }

        Debug.Log("NORMALISED = " + direction.normalized);
        ChangeGroundedDirection( direction.normalized);
    }


    public bool CheckGround()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(_groundedRaycastTransform.position, _groundedDirection, out hit, .055f, _groundMask))
        {
            _isGrounded = true;
            _groundedDirection = -hit.normal;
        }
        else
        {
            _isGrounded = false;
        }
        Debug.DrawLine(_groundedRaycastTransform.position, _groundedRaycastTransform.position + _groundedDirection * .055f, Color.red);
        return _isGrounded;
    }

    //Utilisé uniquement en crawlingMovement
    public void CheckInFront()
    {
        //CHECK DEVANT
        RaycastHit hit;
        if (Physics.Raycast(_groundedRaycastTransform.position, _groundedRaycastTransform.forward, out hit, .055f, _groundMask))
        {
            SwitchPlane(hit);
        }
        else
        {
            if (!Physics.Raycast(_forwardGroundedRaycastTransform.position, _groundedDirection, out hit, .055f, _groundMask))
            {
                if (Physics.Raycast(_belowGroundedRaycastTransform.position, -_belowGroundedRaycastTransform.forward, out hit, .055f, _groundMask))
                {
                    SwitchPlane(hit);
                }
            }
        }

        Debug.DrawLine(_groundedRaycastTransform.position, _groundedRaycastTransform.position + _groundedRaycastTransform.forward * .055f, Color.green, 1f);
        Debug.DrawLine(_forwardGroundedRaycastTransform.position, _forwardGroundedRaycastTransform.position + _groundedDirection * .055f, Color.green, 1f);
        Debug.DrawLine(_belowGroundedRaycastTransform.position, _belowGroundedRaycastTransform.position - _belowGroundedRaycastTransform.forward * .055f, Color.green, 1f);
    }

    public void ChangeGroundedDirection(Vector3 direction)
    {
        _groundedDirection = direction;
    }

    public void PrepareNextGroundedDirection(Vector3 direction)
    {
        _previousGroundedDirection = _groundedDirection;
        _nextGroundedDirection = direction;
    }

    private void SwitchPlane(RaycastHit hit)
    {

        PrepareNextGroundedDirection(-hit.normal);
        //ChangeGroundedDirection(-hit.normal);
        //SetGroundedDirection();
        _crawlingMovement.SetUpTransition(hit.point, hit.normal);
    }
}
