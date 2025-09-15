using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrawlingMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GroundChecker _groundChecker;
    [SerializeField]
    private CharacterMovement _characterMovement;
    [SerializeField]
    private Foot_GroundCheck[] _feetGroundChecks;
    [SerializeField]
    public Body_IK _bodyIK;

    [Header("Values")]
    [SerializeField]
    private float _movementSpeed;
    [SerializeField]
    private float _bodyHeight = .15f;
    [SerializeField]
    private float _bodyHeightSpeed = 7f;
    [SerializeField]
    private float _rotationSpeed = 7f;



    [Header("DEBUG")]
    [SerializeField]
    private Transform _GROUNDPOSDEBUG;

    //This function should handle Everything about Crawling Movement ==> To call in Update
    public void HandleCrawlingMovement(Vector2 _moveValue)
    {
        //Check si on doit changer de mouvement
        if (_characterMovement._isJumpingCreature && IsVaguelyHorizontal())
        {
            _characterMovement.SwitchToJumpingovement();
        }

        //Get the average of leg positions and normals
        Vector3 groundedDirection = Vector3.zero;
        Vector3 groundedPosition = Vector3.zero;

        foreach(Foot_GroundCheck foot in _feetGroundChecks)
        {
            groundedDirection += foot.GetDestination().normal;
            groundedPosition += foot.GetDestination().position;
        }
        groundedDirection = (groundedDirection/_feetGroundChecks.Length).normalized;
        groundedPosition = groundedPosition / _feetGroundChecks.Length;

        //Set in ground Checker
        _groundChecker._groundedDirection = -groundedDirection;
        _groundChecker._groundedPos = groundedPosition;
        _GROUNDPOSDEBUG.position = groundedPosition;

        //Move according to previous data
        HandleMoving(_moveValue, groundedDirection);

        //_bodyIK.RotateHead(_characterMovement._mainCamera.transform.forward, transform.up);
    }

    private void HandleMoving(Vector2 moveValue, Vector3 newUp)
    {
        if (_characterMovement.CheckMoving())
        {
            Debug.Log("Character MOVING " + _characterMovement.name);
            transform.rotation = Quaternion.Slerp(transform.rotation, CalculateNewRotation(newUp), Time.deltaTime * _rotationSpeed);
            Vector3 movementOffset = (transform.forward * moveValue.y + transform.right * moveValue.x).normalized;

            Vector3 heightOffset = _groundChecker._groundedPos + transform.up * _bodyHeight;

            transform.position = Vector3.Lerp(transform.position, heightOffset, Time.deltaTime * _bodyHeightSpeed);

            transform.position = Vector3.Lerp(transform.position, transform.position + movementOffset , Time.deltaTime * _movementSpeed);
            if (_bodyIK != null)
            {
                _bodyIK.BobHead();
                _bodyIK.ThoseHipsDontLie();
            }

        }
    }


    private Quaternion CalculateNewRotation(Vector3 newUp)
    {
        //Projection du forward de la camera
        Vector3 camForwardProj;
        float dotProduct = Vector3.Dot(_characterMovement._mainCamera.transform.forward, -newUp);
        //Debug.Log("DOT PRODUCT : " + dotProduct);
        if (dotProduct > .3f)
        {
            camForwardProj = Vector3.ProjectOnPlane(_characterMovement._mainCamera.transform.up, newUp).normalized;
        }
        else
        {
            camForwardProj = Vector3.ProjectOnPlane(_characterMovement._mainCamera.transform.forward, newUp).normalized;
        }

        //Recalcul du vecteur Right
        Vector3 calculatedRight = Vector3.Cross(newUp, camForwardProj).normalized;

        Vector3 newForward = Vector3.Cross(calculatedRight, newUp).normalized;
        Quaternion targetRot = Quaternion.LookRotation(newForward, newUp);
        return targetRot;
    }

    public bool IsVaguelyHorizontal()
    {
        //RaycastHit hit;
        //if(Physics.Raycast(_groundChecker._ray.position, ))

        bool horizontal = false;
        float collisionAngle = Vector3.Angle(-_groundChecker._groundedDirection, Vector3.up);
        Debug.Log("CRAWLING COLLISION ANGLE = " + collisionAngle);
        if (collisionAngle < 50)
        {
            horizontal = true;
        }
        //Debug.Log("Dot : " + Vector3.Dot(-_groundChecker._groundedDirection, Vector3.up));
        return horizontal;
    }
}
