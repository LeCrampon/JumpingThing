using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FernIkMovement : MonoBehaviour
{
    [SerializeField]
    private float _movementAmplitude;
    [SerializeField]
    private float _movementSpeed;

    private Vector3 _startPosition;
    private float _stepLerp;

    private void Awake()
    {
        _startPosition = transform.localPosition;
    }

    private void BobFernUpAndDown()
    {
        if (_stepLerp >= 1)
        {
            _stepLerp = 0;
        }
        float newY = Mathf.Sin(_stepLerp * Mathf.PI) * _movementAmplitude;
        transform.localPosition = new Vector3(transform.localPosition.x, _startPosition.y + newY, transform.localPosition.z);
        _stepLerp += Time.deltaTime * _movementSpeed;

    }

    private void Update()
    {
        BobFernUpAndDown();
    }
}
