using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSTracker : MonoBehaviour
{
    private TMP_Text _fpsText;
    private float _fps = 0;
    private float _timer = 0;
    private int _frameCount = 0;

    private void Awake()
    {
        _fpsText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        _timer += Time.deltaTime;
        _frameCount++;

        if (_timer >= 1f)
        {
            _fps = _frameCount / _timer;
            _fpsText.text = "FPS: " + _fps.ToString("F1");
            _timer = 0;
            _frameCount = 0;
        }
    }
}
