using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTrample : MonoBehaviour
{
    [SerializeField]
    private Material _grassMat;
    [SerializeField]
    private float _radius;
    [SerializeField]
    private float _heightOffset;

    private Transform _cachedTransform;
    private int _grassTrampleProperty = Shader.PropertyToID("_Trample");

    private void Awake()
    {
        _cachedTransform = transform;
    }

    private void Update()
    {
        if(_grassMat == null)
        {
            return;
        }

        Vector3 position = _cachedTransform.position;
        _grassMat.SetVector(_grassTrampleProperty, new Vector4(position.x, position.y + _heightOffset, position.z, _radius));
    }
}
