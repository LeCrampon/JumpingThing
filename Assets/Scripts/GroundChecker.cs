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
    [SerializeField]
    public Vector3 _groundedPos = Vector3.zero;
    [SerializeField]
    private float _groundedRaycastLength;

    [Header("TESTINg SOMETHING")]
    [SerializeField]
    private Transform _feetPositions;




    public bool CheckGround()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(_groundedRaycastTransform.position, _groundedDirection, out hit, _groundedRaycastLength, _groundMask))
        {
            _isGrounded = true;
            _groundedDirection = -hit.normal;
            _groundedPos = hit.point;
        }
        else
        {
            _isGrounded = false;
        }
        Debug.DrawLine(_groundedRaycastTransform.position, _groundedRaycastTransform.position + _groundedDirection * _groundedRaycastLength, Color.red);
        return _isGrounded;
    }

    public bool CrawlingCheckGeound()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(_groundedRaycastTransform.position, _groundedDirection, out hit, _groundedRaycastLength, _groundMask))
        {
            _isGrounded = true;
            _groundedPos = hit.point;
        }
        else
        {
            _isGrounded = false;
        }
        Debug.DrawLine(_groundedRaycastTransform.position, _groundedRaycastTransform.position + _groundedDirection * _groundedRaycastLength, Color.red);
        return _isGrounded;
    }

    //Utilisé uniquement en crawlingMovement
    public void CheckInFront()
    {
        //CHECK DEVANT
        RaycastHit hit;
        if (Physics.Raycast(_groundedRaycastTransform.position, _groundedRaycastTransform.forward, out hit, _groundedRaycastLength, _groundMask))
        {
            SwitchPlane(hit);
        }
        else
        {
            if (!Physics.Raycast(_forwardGroundedRaycastTransform.position, _groundedDirection, out hit, _groundedRaycastLength, _groundMask))
            {
                if (Physics.Raycast(_belowGroundedRaycastTransform.position, -_belowGroundedRaycastTransform.forward, out hit, _groundedRaycastLength, _groundMask))
                {
                    SwitchPlane(hit);
                }
            }
        }

        Debug.DrawLine(_groundedRaycastTransform.position, _groundedRaycastTransform.position + _groundedRaycastTransform.forward * _groundedRaycastLength, Color.green, 1f);
        Debug.DrawLine(_forwardGroundedRaycastTransform.position, _forwardGroundedRaycastTransform.position + _groundedDirection * _groundedRaycastLength, Color.green, 1f);
        Debug.DrawLine(_belowGroundedRaycastTransform.position, _belowGroundedRaycastTransform.position - _belowGroundedRaycastTransform.forward * _groundedRaycastLength, Color.green, 1f);
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

    private void Update()
    {
        GravityManagement();
    }

    private void GravityManagement()
    {

        //Vector3 newPos = Vector3.Lerp(transform.position, _groundedPos - _groundedDirection.normalized * .2f, Time.deltaTime*10);

    }
}
