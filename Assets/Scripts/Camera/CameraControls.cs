using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    private float _xAngle, _yAngle;
    private Vector2 _lookValue;

    [Header("References")]
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private GroundChecker _groundChecker;

    [Header("Rotation Limits")]
    [SerializeField]
    private float _maxGroundYLimit = 90f;
    [SerializeField]
    private float _minGroundYLimit = -5f;


    [SerializeField]
    private float _minWallYLimit = 100;
    [SerializeField]
    private float _maxWallYLimit = 80;

    [SerializeField]
    private float _currentMinYLimit;
    [SerializeField]
    private float _currentMaxYLimit;


    [Header("Speed and Lerp values")]
    [SerializeField]
    private float _cameraSpeed;
    [SerializeField]
    private float _cameraSmoothingFactor;

    [SerializeField]
    private CharacterMovement _characterMovement;




    private void Awake()
    {
        _xAngle = transform.localRotation.eulerAngles.x;
        _yAngle = transform.localRotation.eulerAngles.y;
    }

    private void Update()
    {
        if (_characterMovement._movementType == MovementType.CrawlingMovement /*|| _characterMovement._movementType == MovementType.JumpingMovement*/)
        {
            //UpdateAngleLimits();
            if (_groundChecker._niveauABulles == NiveauABulles.Ground)
            {
                CrawlingRotateCameraGround();
            }
            else if (_groundChecker._niveauABulles == NiveauABulles.Wall)
            {
                CrawlingRotateCameraWall();
            }
            else
            {
                CrawlingRotateCameraWall();
            }
        }
        else if (_characterMovement._movementType == MovementType.JumpingMovement)
        {
            JumpingRotateCamera();
            CrawlingRotateCameraGround();
        }
        else if (_characterMovement._movementType == MovementType.FlyingMovement)
        {
            CrawlingRotateCameraGround();
        }
    }

    public void JumpingRotateCamera()
    {
        //Calculate new verticalLimits:
        Vector3 normal = -_groundChecker._groundedDirection;
        float angle = Vector3.Angle(_groundChecker.transform.forward, Vector3.up) - 90;


        ///TEST LIMITS
        Vector3 horizontalForward = -Vector3.Cross(Vector3.up, transform.right);

        float dot = Vector3.Dot(horizontalForward, _groundChecker.transform.forward);
        dot = dot < 0 ? -1 : 1;

        _currentMinYLimit = _minGroundYLimit + angle * dot;
        _currentMaxYLimit = _maxGroundYLimit + angle * dot;


        float horizontalInput = Mathf.Lerp(0, _lookValue.x , Time.deltaTime * _cameraSmoothingFactor);
        float verticalInput = Mathf.Lerp(0, -_lookValue.y , Time.deltaTime * _cameraSmoothingFactor);

        _xAngle += verticalInput * _cameraSpeed * Time.deltaTime;
        _yAngle += horizontalInput * _cameraSpeed * Time.deltaTime;

        _xAngle = Mathf.Clamp(_xAngle, _currentMinYLimit, _currentMaxYLimit);


        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_xAngle, _yAngle, 0), Time.deltaTime * _cameraSmoothingFactor);
    }

    public void SetLookValue(Vector2 lookValue)
    {
        _lookValue = lookValue * GameStateManager._instance.GetMouseSensitivity();
        //Debug.Log("Look Value "  + _lookValue);
    }

    public Vector2 GetLookValue()
    {
        return _lookValue;
    }

    void CrawlingRotateCameraGround()
    {
        //Calculate new verticalLimits:
        Vector3 normal = -_groundChecker._groundedDirection;
        float angle = Vector3.Angle(_groundChecker.transform.forward, Vector3.up) -90;


        ///TEST LIMITS
        Vector3 horizontalForward = -Vector3.Cross(Vector3.up, transform.right);

        float dot = Vector3.Dot(horizontalForward, _groundChecker.transform.forward);
        dot = dot < 0 ? -1 : 1;

        _currentMinYLimit = _minGroundYLimit + angle * dot;
        _currentMaxYLimit = _maxGroundYLimit + angle * dot;


        float horizontalInput = Mathf.Lerp(0, _lookValue.x, Time.deltaTime * _cameraSmoothingFactor);
        float verticalInput = Mathf.Lerp(0, -_lookValue.y, Time.deltaTime * _cameraSmoothingFactor);

        _xAngle += verticalInput * _cameraSpeed * Time.deltaTime;
        _yAngle += horizontalInput * _cameraSpeed * Time.deltaTime;

        _xAngle = Mathf.Clamp(_xAngle, _currentMinYLimit, _currentMaxYLimit);


        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_xAngle, _yAngle, 0), Time.deltaTime * _cameraSmoothingFactor);
    }

    private void CrawlingRotateCameraWall()
    {
        float horizontalInput = Mathf.Lerp(0, _lookValue.x, Time.deltaTime * _cameraSmoothingFactor);
        float verticalInput = Mathf.Lerp(0, -_lookValue.y, Time.deltaTime * _cameraSmoothingFactor);

        _xAngle += verticalInput * _cameraSpeed * Time.deltaTime;
        _yAngle += horizontalInput * _cameraSpeed * Time.deltaTime;

        //On calcule l'angle de la normale du plan
        float normalY = Mathf.Atan2(_groundChecker._groundedDirection.normalized.x, _groundChecker._groundedDirection.normalized.z) * Mathf.Rad2Deg;
        _currentMinYLimit = normalY - _minWallYLimit;
        _currentMaxYLimit = normalY + _maxWallYLimit;

        _yAngle = ClampAngle(_yAngle, _currentMinYLimit, _currentMaxYLimit);
        _xAngle = ClampAngle(_xAngle, -90, 90);

        //transform.localRotation = Quaternion.Euler(_xAngle, _yAngle, 0);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_xAngle, _yAngle, 0), Time.deltaTime * _cameraSmoothingFactor);
    }

    //Fonction de clamp qui prend en compte le cycle 360°
    float ClampAngle(float angle, float min, float max)
    {
        //Ramene l'angle et les limites dans les valeurs [0, 360]
        angle = Mathf.Repeat(angle, 360f);
        min = Mathf.Repeat(min, 360f);
        max = Mathf.Repeat(max, 360f);

        //Si l'angle est dans l'intervalle, on ne clamp pas
        if (IsAngleInInterval(angle, min, max))
            return angle;

        // On calcule les distance entre l'angle et le min et le max avec DeltaAngle (prend en compte le sens de rotation)
        // On prend la valeur absolue (une distance non signée)
        float distToMin = Mathf.Abs(Mathf.DeltaAngle(angle, min));
        float distToMax = Mathf.Abs(Mathf.DeltaAngle(angle, max));
        
        //On checke de quelle borne on est le plus proche.
        if(distToMin < distToMax)
        {
            return min;
        }
        else
        {
            return max;
        }
    }

    // Verifie que l'angle est dans l'intervalle [start, end]
    bool IsAngleInInterval(float a, float start, float end)
    {
        if (start <= end)
            return a >= start && a <= end;
        else
            return a >= start || a <= end;
    }
}
