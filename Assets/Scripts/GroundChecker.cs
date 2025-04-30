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

    [SerializeField]
    public NiveauABulles _niveauABulles;


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


    public void ChangeGroundedDirection(Vector3 direction)
    {
        _groundedDirection = direction;
    }

    public void PrepareNextGroundedDirection(Vector3 direction)
    {
        _previousGroundedDirection = _groundedDirection;
        _nextGroundedDirection = direction;
    }


    public void SetNiveauABulles()
    {
        float angle = Vector3.Angle(_groundedDirection, Vector3.down);
        if (angle <= 70)
        {
            _niveauABulles = NiveauABulles.Ground;
        }
        else if(angle > 70 && angle < 120)
        {
            _niveauABulles = NiveauABulles.Wall;
        }
        else
        {
            _niveauABulles = NiveauABulles.UpsideDown;
        }
    }

    private void Update()
    {
        SetNiveauABulles();
    }
}

public enum NiveauABulles
{
    Ground,
    Wall,
    UpsideDown
}
