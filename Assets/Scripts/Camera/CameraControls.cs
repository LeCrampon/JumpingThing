using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    private float _xAngle, _yAngle;

    [SerializeField]
    private float _upperVerticalLimit = 90f;
    [SerializeField]
    private float _lowerVerticalLimit = -5f;


    //[SerializeField]
    //private float _upGroundLimit = 35f;
    //[SerializeField]
    //private float _lowGroundLimit = 35f;

    //[SerializeField]
    //private float _upWallLimit = 100;
    //[SerializeField]
    //private float _lowWallLimit = 80;

    //[SerializeField]
    //private float _upUpsideDownLimit = 80;
    //[SerializeField]
    //private float _lowUpsideDownLimit = 100;

    [SerializeField]
    private float _cameraSpeed;
    [SerializeField]
    private float _cameraSmoothingFactor;
    [SerializeField]
    private GroundChecker _groundChecker;

    private Camera _cam;

    private Vector2 _lookValue;

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float newLowLimit;
    [SerializeField]
    private float newUpLimit;

    private void Awake()
    {
        _cam = GetComponentInChildren<Camera>();

        _xAngle = transform.localRotation.eulerAngles.x;
        _yAngle = transform.localRotation.eulerAngles.y;
    }

    private void Update()
    {
        //UpdateAngleLimits();
        if(_groundChecker._niveauABulles == NiveauABulles.Ground)
        {
            RotateCamera();
        }
        else if(_groundChecker._niveauABulles == NiveauABulles.Wall)
        {
            RotateCameraWall();
        }
        //RotateCamera();
        //RotateCameraUpsideDown();
    }

    public void SetLookValue(Vector2 lookValue)
    {
        _lookValue = lookValue;
    }

    //private void UpdateAngleLimits()
    //{
    //    switch (_groundCheck._niveauABulles)
    //    {
    //        case NiveauABulles.Ground:
    //            SetAngleLimits(_lowGroundLimit, _upGroundLimit);
    //            break;
    //        case NiveauABulles.Wall:
    //            SetAngleLimits(_lowWallLimit, _upWallLimit);
    //            break;
    //        case NiveauABulles.UpsideDown:
    //            SetAngleLimits(_lowUpsideDownLimit, _upUpsideDownLimit);
    //            break;
    //        default:
    //            SetAngleLimits(_lowGroundLimit, _upGroundLimit);
    //            break;
    //    }
    //}

    private void SetAngleLimits(float _lowLimit, float _upLimit)
    {
        _lowerVerticalLimit = Mathf.Lerp(_lowerVerticalLimit, _lowLimit, 5 * Time.deltaTime);
        _upperVerticalLimit = Mathf.Lerp(_upperVerticalLimit, _upLimit, 5 * Time.deltaTime);
    }

    void RotateCamera()
    {
        //TODO : CLEAN THIS BULLSHIT
        //TODO : CLEAN THIS BULLSHIT
        //TODO : CLEAN THIS BULLSHIT
        //TODO : CLEAN THIS BULLSHIT
        //TODO : CLEAN THIS BULLSHIT

        //Calculate new verticalLimits:
        Vector3 normal = -_groundChecker._groundedDirection;
        float angle = Vector3.Angle(_groundChecker.transform.forward, Vector3.up) -90;


        ///TEST LIMITS
        Vector3 horizontalForward = -Vector3.Cross(Vector3.up, transform.right);

        float dot = Vector3.Dot(horizontalForward, _groundChecker.transform.forward);
        dot = dot < 0 ? -1 : 1;
        //Debug.Log("angle " + angle + " / dot " + dot);

        //TODO : MAYBE LERP LA LIMITE ????
         newLowLimit = _lowerVerticalLimit + angle * dot;
         newUpLimit = _upperVerticalLimit + angle * dot;

        //Debug.Log("NEW LIMITS " + newLowLimit + "/" + newUpLimit);

        float horizontalInput = Mathf.Lerp(0, _lookValue.x, Time.deltaTime * _cameraSmoothingFactor);
        float verticalInput = Mathf.Lerp(0, -_lookValue.y, Time.deltaTime * _cameraSmoothingFactor);

        _xAngle += verticalInput * _cameraSpeed * Time.deltaTime;
        _yAngle += horizontalInput * _cameraSpeed * Time.deltaTime;

        //Debug.Log("xAngle : " + _xAngle);
        _xAngle = Mathf.Clamp(_xAngle, newLowLimit, newUpLimit);
        //Debug.Log("Clamped xAngle : " + _xAngle);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_xAngle, _yAngle, 0), Time.deltaTime * _cameraSmoothingFactor);
    }

    private void RotateCameraWall()
    {

        ///// TODO TODO TODO TODO
        ///// TODO TODO TODO TODO
        ///// TODO TODO TODO TODO
        ///// TODO TODO TODO TODO
        ///// TODO TODO TODO TODO
        ///// TODO TODO TODO TODO
        /// GERER ROTATION

        float newYLowLimit = ConvertLocalYToWorldY(-90f, -_groundChecker._groundedDirection);
        float newYUpLimit = ConvertLocalYToWorldY(90f, -_groundChecker._groundedDirection);

        

        //SWAP
       


        float horizontalInput = Mathf.Lerp(0, _lookValue.x, Time.deltaTime * _cameraSmoothingFactor);
        float verticalInput = Mathf.Lerp(0, -_lookValue.y, Time.deltaTime * _cameraSmoothingFactor);

        _xAngle += verticalInput * _cameraSpeed * Time.deltaTime;
        _yAngle += horizontalInput * _cameraSpeed * Time.deltaTime;

        //Swap + add 180
        if (newYLowLimit > newYUpLimit)
        {
            ////newYUpLimit += 360f;
            //float temp = newYLowLimit;
            //newYLowLimit = newYUpLimit + 180;
            //newYUpLimit = temp + 180;
            newYUpLimit += 180f;
            newYLowLimit -= 180f;
        }
        Debug.Log("LOW " + newYLowLimit);
        Debug.Log("HIGH " + newYUpLimit);

        //Debug.Log("xAngle : " + _xAngle);
        _xAngle = Mathf.Clamp(_xAngle, -90, 90);
        //_yAngle = Mathf.Clamp(_yAngle, newYLowLimit, newYUpLimit);
        _yAngle = Mathf.Clamp(_yAngle, newYLowLimit, newYUpLimit);
        //Debug.Log("Clamped xAngle : " + _xAngle);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_xAngle, _yAngle, 0), Time.deltaTime * _cameraSmoothingFactor);
    }

    public float AngleAddition(float firstAngle, float secondAngle)
    {
        float calculatedAngle = firstAngle + secondAngle;
        if (calculatedAngle > 180)
            calculatedAngle -= 180;
        else if (calculatedAngle < -180)
            calculatedAngle += 180;

        return calculatedAngle;
    }

    public float NormalizeAngle(float angle)
    {
        angle = angle % 360f;
        Debug.Log("MODULO " + angle);
        if(angle > 180f)
        {
            angle -= 360f;
        }
        else if (angle <=  -180f)
        {
            angle += 360f;
        }
        Debug.Log("Corrected Angle " + angle);
        return angle;
    }


    public float ConvertLocalYToWorldY(float localAngleY, Vector3 wallNormal)
    {
        // 1. Calculer l'axe Y local du mur (son "haut")
        Vector3 wallYAxis = wallNormal; // Suppose que wallNormal = "haut" du mur

        // 2. Calculer l'avant du mur (perpendiculaire à sa normale)
        Vector3 wallForward = Vector3.Cross(wallYAxis, _groundChecker.transform.right).normalized;
        if (wallForward.magnitude < 0.1f) // Cas particulier pour les murs nord/sud
            wallForward = Vector3.Cross(wallYAxis, _groundChecker.transform.forward).normalized;

        // 3. Générer la rotation locale
        Quaternion localRot = Quaternion.AngleAxis(localAngleY, wallYAxis);
        Vector3 rotatedForward = localRot * wallForward;

        // 4. Projeter sur le plan horizontal monde
        Vector3 worldProjected = Vector3.ProjectOnPlane(rotatedForward, Vector3.up).normalized;

        // 5. Calculer l'angle Y monde
        float worldY = Vector3.SignedAngle(Vector3.forward, worldProjected, Vector3.up);

        //normaliser
        //worldY = worldY * Mathf.Floor((worldY + 180f) / 180f);

        return worldY;
    }
}
