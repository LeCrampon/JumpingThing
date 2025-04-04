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
    public Vector3 _nextGroundedDirection = Vector3.down;


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

    private void SwitchPlane(RaycastHit hit)
    {
        ChangeGroundedDirection(-hit.normal);
        _crawlingMovement.SetUpTransition(hit.point, hit.normal);
    }
}
