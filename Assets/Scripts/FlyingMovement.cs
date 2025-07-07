using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FlyingMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CharacterMovement _characterMovement;
    [SerializeField]
    private GroundChecker _groundChecker;

    [Header("Detection")]
    [SerializeField]
    private LayerMask _groundLayer;
    [SerializeField]
    private float _raycastDistance =.5f;

    [Header("Movement Values")]
    [SerializeField]
    private float _forwardSpeed = 5f;
    private float _strafeSpeed = 2f;
    

    public void HandleFlyingMovement(Vector2 moveValue)
    {
        transform.forward = _characterMovement._mainCamera.transform.forward;

        Vector3 movementOffset = (transform.forward * moveValue.y + transform.right * moveValue.x).normalized;

        transform.position = Vector3.Lerp(transform.position, transform.position + movementOffset, Time.deltaTime * _forwardSpeed);

        DetectWalls();

    }

    private void DetectWalls()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red, 1f);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward,  out hit, _raycastDistance, _groundLayer))
        {
            transform.up = hit.normal;
        }
    }
}
