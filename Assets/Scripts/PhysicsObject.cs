using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [SerializeField]
    private Rigidbody _rigidBody;
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private float _distanceCamera;

    private void Start()
    {
        _camera = GameStateManager._instance.GetMainCamera();
    }

    private void Update()
    {
        ActivateCamera();
    }

    private void ActivateCamera()
    {
        if(Vector3.Distance(_camera.transform.position, transform.position) > _distanceCamera)
        {
            _rigidBody.isKinematic = true;
        }
        else
        {
            _rigidBody.isKinematic = false;
        }
    }
}
