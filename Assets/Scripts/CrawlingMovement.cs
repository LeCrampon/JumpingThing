using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlingMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GroundChecker _groundChecker;
    [SerializeField]
    private CharacterMovement _characterMovement;

    [SerializeField]
    private float _movementSpeed;

    [Header("Plane Transition")]
    [SerializeField]
    private bool _inPlaneTransition;
    private Vector3 _planeTransitionDestination;
    private Vector3 _planeTransitionStart;
    private Quaternion _planeTransitionOldRotation;
    private Quaternion _planeTransitionNewRotation;
    [SerializeField]
    private float _switchDuration;
    [SerializeField]
    private float _switchTimer;

    [SerializeField]
    private Foot_GroundCheck[] _groundChecks;

    [Header("DEBUG")]
    [SerializeField]
    private Transform _GROUNDPOSDEBUG;

    //This function should handle Everything about Crawling Movement ==> To call in Update
    public void OLD_HandleCrawlingMovement(Vector2 _moveValue)
    {
        //if (!_inPlaneTransition)
        //{
        //    _groundChecker.CheckInFront();
        //    HandleMoving(_moveValue);
        //}
        //else
        //{
        //    HandlePlaneTransition();
        //}
        if (!_inPlaneTransition)
        {
            _groundChecker.CheckInFront();

            HandleMoving(_moveValue);
        }

        if(_inPlaneTransition)
        {
            HandleMovingTransition(_moveValue);
            HandlePlaneTransition();
        }

    }

    public void HandleCrawlingMovement(Vector2 _moveValue)
    {

        Vector3 groundedDirection = Vector3.zero;
        Vector3 groundedPosition = Vector3.zero;

        foreach(Foot_GroundCheck foot in _groundChecks)
        {
            groundedDirection += foot.GetDestination().normal;
            groundedPosition += foot.GetDestination().position;
        }
        groundedDirection = (groundedDirection/_groundChecks.Length).normalized;
        groundedPosition = groundedPosition / _groundChecks.Length;
        //Debug.Log("GroundedDirection " + groundedDirection);


        _groundChecker._groundedDirection = -groundedDirection;
        _groundChecker._groundedPos = groundedPosition;
        _GROUNDPOSDEBUG.position = groundedPosition;

        HandleMovingNEW(_moveValue, groundedDirection);
    }


    private void HandleMoving(Vector2 moveValue)
    {
        if (/*_groundChecker.CheckGround() && */_characterMovement.CheckMoving())
        {
            transform.rotation = CalculateNewRotation(-_groundChecker._groundedDirection);
            Vector3 mouvement = (transform.forward * moveValue.y + transform.right * moveValue.x).normalized;
            transform.position += mouvement * _movementSpeed * Time.deltaTime;
        }
    }

    private void HandleMovingTransition(Vector2 moveValue)
    {
        if (_characterMovement.CheckMoving())
        {
            //transform.rotation = CalculateNewRotation(-_groundChecker._groundedDirection);
            Vector3 mouvement = (transform.forward * moveValue.y + transform.right * moveValue.x).normalized;
            transform.position += mouvement * _movementSpeed * Time.deltaTime;
        }
    }

    private void HandleMovingNEW(Vector2 moveValue, Vector3 newUp)
    {
        if (_characterMovement.CheckMoving() /*&& _groundChecker.CheckGround()*/)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,CalculateNewRotation( newUp), Time.deltaTime * 7);
            Vector3 movementOffset = (transform.forward * moveValue.y + transform.right * moveValue.x).normalized;
            //transform.position += movementOffset * _movementSpeed * Time.deltaTime;
            //Vector3 heightOffset = Vector3.zero;
            //if(Vector3.Distance(transform.position, _groundChecker._groundedPos) < .1f)
            //{
            //    heightOffset = transform.up;
            //}
            //else if(Vector3.Distance(transform.position, _groundChecker._groundedPos) > .1f)
            //{
            //    heightOffset = -transform.up;
            //}


            Vector3 heightOffset = _groundChecker._groundedPos + transform.up * .15f;

            transform.position = Vector3.Lerp(transform.position, heightOffset, Time.deltaTime * 7);

            transform.position = Vector3.Lerp(transform.position, transform.position + movementOffset , Time.deltaTime * _movementSpeed);
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

    public void SetUpTransition(Vector3 position, Vector3 newUp)
    {
        _planeTransitionStart = transform.position;
        _planeTransitionOldRotation = transform.rotation;
        _planeTransitionDestination = position;
        _planeTransitionNewRotation = CalculateNewRotation(newUp);
        _inPlaneTransition = true;
    }

    private void HandlePlaneTransition()
    {
        //if (_switchTimer < _switchDuration)
        //{
        //    transform.position = Vector3.Lerp(_planeTransitionStart, _planeTransitionDestination, _switchTimer / _switchDuration);
        //    transform.rotation = Quaternion.Slerp(_planeTransitionOldRotation, _planeTransitionNewRotation, _switchTimer / _switchDuration);
        //    _switchTimer += Time.deltaTime;
        //}
        //else
        //{
        //    _inPlaneTransition = false;
        //    _switchTimer = 0;
        //}

        float maxDistance = Vector3.Distance(_planeTransitionStart, _planeTransitionDestination);
        float currentDistance = Vector3.Distance(transform.position, _planeTransitionStart);

        if (currentDistance < maxDistance)
        {
            //transform.position = Vector3.Lerp(_planeTransitionStart, _planeTransitionDestination, currentDistance / maxDistance);
            _groundChecker._groundedDirection = Vector3.Lerp(_groundChecker._previousGroundedDirection, _groundChecker._nextGroundedDirection, currentDistance / maxDistance);
            transform.rotation = Quaternion.Slerp(_planeTransitionOldRotation, _planeTransitionNewRotation, currentDistance / maxDistance);
            _switchTimer += Time.deltaTime;
        }
        else
        {
            _inPlaneTransition = false;
            _switchTimer = 0;
        }
    }
}
