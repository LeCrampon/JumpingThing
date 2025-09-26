using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CloverRotation : MonoBehaviour
{
   
    private float _bendStrength =.2f;
    private float _bendTimer = 0f;
    private float _bendMaxTime = .3f;
    private float _goBackTimer = 0f;
    private float _goBackMaxTime = .5f;

    private Quaternion _startRotation;
    private bool _colliding= false;

    private Quaternion _targetRotation;

    //private bool _is

    private void Awake()
    {
        _startRotation = transform.localRotation;
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    //Direction
    //    Vector3 direction = transform.position - collision.transform.position;
    //    transform.rotation = Quaternion.LookRotation(-direction.normalized * _bendStrength);
    //    Debug.Log("Colliding with clover !!!!!!");
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    Vector3 direction = transform.position - other.transform.position;
    //    transform.rotation = Quaternion.LookRotation(direction * _bendStrength);
    //    Debug.Log("Colliding with clover !!!!!!");
    //}

    private void OnTriggerEnter(Collider other)
    {
        _colliding = true;
        _bendTimer = 0f;
    }

    private void OnTriggerStay(Collider other)
    {
        Vector3 direction =  other.transform.position - transform.position ;
        float distance = direction.magnitude;
        direction = -direction.normalized;
        //transform.rotation = Quaternion.LookRotation(-direction * _bendStrength);
        //Debug.Log("Colliding with clover !!!!!!");
        float angleX = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg * _bendStrength;
        float angleZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg * _bendStrength;

        _targetRotation = Quaternion.Euler(angleX, 0f, angleZ);

    }

    private void OnTriggerExit(Collider other)
    {
        //StartCoroutine(GoBackToStartRotation());
        _colliding = false;
        _goBackTimer = 0f;
    }

    private void Update()
    {
        if (_colliding)
        {
            if (_bendTimer < _bendMaxTime)
            {
                _bendTimer += Time.deltaTime;
                transform.localRotation = Quaternion.Slerp(_startRotation, _targetRotation, _bendTimer/_bendMaxTime);
                return;
            }
        }
        else
        {  
            if (_goBackTimer < _goBackMaxTime)
            {
                _goBackTimer += Time.deltaTime;
                transform.localRotation = Quaternion.Slerp(transform.localRotation, _startRotation, _goBackTimer / _goBackMaxTime);
                return;
            }

        }
    }

    //private IEnumerator GoBackToStartRotation()
    //{

    //}
}
