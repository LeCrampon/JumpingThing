using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antenna_IK : MonoBehaviour
{
    [SerializeField]
    private float _stepLerp = 0;
    [SerializeField]
    private float _lerpSpeed;
    [SerializeField]
    private float _wiggleAmplitude;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.localPosition;
    }

    public void WiggleAntenna()
    {
        Vector3 yVariation = new Vector3(0,Mathf.Sin(_stepLerp * Mathf.PI) * _wiggleAmplitude, 0);
        _stepLerp += Time.deltaTime * _lerpSpeed;

        transform.localPosition = _startPosition - yVariation;
    }

    private void Update()
    {
        WiggleAntenna();
    }
}
