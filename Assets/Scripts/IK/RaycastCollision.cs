using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCollision : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _pivot;
    [SerializeField]
    private LayerMask _layerMask;

    [SerializeField]
    private float _colliderRadius;

    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _rotationSpeed;

    private Vector3 _offset;
    private Rigidbody _rigid;



    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _offset = transform.position - _target.position;
    }

    private void FollowTarget()
    {
        Vector3 newOffset = _target.rotation * _offset;
        Vector3 newPosition = _target.position + newOffset;

        Vector3 direction = newPosition - transform.position;
        Debug.DrawRay(transform.position, direction, Color.yellow, .5f);
        if(Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, direction.magnitude, _layerMask))
        {
            Vector3 tempPosition = hit.point - direction.normalized * _colliderRadius;
            if(Vector3.Distance(tempPosition, newPosition) < .1f )
            {
                newPosition = tempPosition;
            }

        }

        _rigid.MovePosition(newPosition);
        _rigid.MoveRotation( _target.rotation);
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        FollowTarget();
    }
}
