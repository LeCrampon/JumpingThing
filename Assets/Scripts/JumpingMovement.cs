using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class JumpingMovement : MonoBehaviour
{
    [Header("Movement Bools")]
    [SerializeField]
    private bool _isJumping;

    [Header("Jump Parameters")]
    [SerializeField]
    private float _jumpHeight;
    [SerializeField]
    private float _jumpLength;
    [SerializeField]
    private float _jumpDuration;
    [SerializeField]
    private AnimationCurve _jumpCurveY;
    [SerializeField]
    private float _jumpTimer;
    private Vector3 _startJumpPos;
    private Quaternion _startJumpRot;
    private Vector3 _endJumpPos;
    private Vector3 _collisionJumpPos;
    private Vector3 _endJumpNormal;
    private Quaternion _endJumpRot;
    private float _currentJumpDuration;

    [Header("RayCast")]
    [SerializeField]
    private LayerMask _groundMask;
    [SerializeField]
    private LayerMask _wallMask;

    [Header("References")]
    [SerializeField]
    private Camera _mainCamera;
    [SerializeField]
    private GameObject _playerModel;
    [SerializeField]
    private CharacterMovement _characterMovement;
    [SerializeField]
    private GroundChecker _groundChecker;
    [SerializeField]
    private JumpingAudio _jumpingAudio;
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private Rig _feetRig;


    [Header("DEBUG")]
    [SerializeField]
    private Transform _dStartJumpTransform;
    [SerializeField]
    private Transform _dEndJumpTransform;
    [SerializeField]
    private Transform _dTempJumpTransform;
    [SerializeField]
    private Transform _dJumpCollision;
    [SerializeField]
    private Transform[] _dTrajectory;

    private bool _isGoingToWall = false;

    [SerializeField]
    private float _currentHeight;



    private void Start()
    {
        _currentHeight = _jumpHeight;
        _currentJumpDuration = _jumpDuration;
        float jumpMult = 1 / (_currentJumpDuration / 2);
        _animator.SetFloat("StartJumpMultiplier", jumpMult);
        _animator.SetFloat("EndJumpMultiplier", jumpMult);
    }

    public Vector3 GetStartJumpPos()
    {
        return _startJumpPos;
    }


    public float GetJumpTimer()
    {
        return _jumpTimer ;
    }

    public float GetJumpDuration()
    {
        return _jumpDuration;
    }

    public float GetCurrentJumpDuration()
    {
        return _currentJumpDuration;
    }

    public Vector3 GetEndJumpPos()
    {
        return _endJumpPos;
    }

    //Should handle everything related to jumping movement, to use in Update
    public void HandleJumpingMovement(Vector2 moveValue)
    {
        HandleMoveJumping(moveValue);
        HandleJump();
    }

    private void SetEndJumpRotationCollision()
    {
        _endJumpRot = CalculateRotationFromNormal(_endJumpNormal);
    }

    public void SetStartJumpRotation()
    {
        transform.LookAt(new Vector3(_endJumpPos.x, transform.position.y, _endJumpPos.z));
    }

    private void SetEndJumpRotation()
    {
        _endJumpRot = CalculateRotationFromNormal(_endJumpNormal);
    }


    private Quaternion CalculateRotationFromNormal(Vector3 normal)
    {
        Vector3 newForward = _mainCamera.transform.forward;
        newForward = Vector3.ProjectOnPlane(newForward, normal).normalized;
        Vector3 newRight = Vector3.Cross(normal, newForward).normalized;
        newForward = Vector3.Cross(newRight, normal).normalized;

        Debug.DrawLine(transform.position, transform.position + newForward, Color.blue, 10f);
        Debug.DrawLine(transform.position, transform.position + newRight, Color.red, 10f);
        Debug.DrawLine(transform.position, transform.position + normal, Color.green, 10f);

        //Debug.Log("New Forward " + newForward);
        //Debug.Log("Normal " + normal);
        Quaternion quat = Quaternion.LookRotation(newForward, normal);

        return quat;
    }
    private void ManageJumpRotation()
    {
        transform.rotation = Quaternion.Slerp(_startJumpRot, _endJumpRot, _jumpTimer / _jumpDuration);
    }


    #region JumpManagement
    private void FindJumpDestination(Vector2 moveValue)
    {
        _startJumpPos = transform.position;
        _startJumpRot = transform.rotation;
        //on set de base la _endJumpNormal à Vector3.up
        _endJumpNormal = Vector3.up;
        Vector3 forward = (_mainCamera.transform.forward * moveValue.y + _mainCamera.transform.right * moveValue.x).normalized;
        forward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;

        

        Vector3 tempPositition = _startJumpPos + forward * _jumpLength;
        tempPositition += Vector3.up * _jumpHeight;
        RaycastHit hit;
        float yPos = 0;
        if(Physics.Raycast(tempPositition, Vector3.down, out hit, 10f, _groundMask))
        {
            yPos = hit.point.y;
            _endJumpNormal = hit.normal;
        }
        _endJumpPos = new Vector3(tempPositition.x, yPos, tempPositition.z) ;

        //SetEndJumpRotation();

        _dEndJumpTransform.position = _endJumpPos;
        _dStartJumpTransform.position = _startJumpPos;
        _dTempJumpTransform.position = tempPositition;
    }

    private void StartJumping()
    {
        _isJumping = true;
        _animator.SetTrigger("StartJump");
        _jumpTimer = 0;
        _jumpingAudio.PlayJumpingAudio();
        _feetRig.weight = 0;
    }

    private void FindJumpDestinationCollision()
    {
        _startJumpPos = transform.position;
        _endJumpPos = _collisionJumpPos;
    }

    private void SetJumpCurve(float height)
    {
        float newHeight = _startJumpPos.y + height;

        _currentJumpDuration = _jumpDuration;
        if(height < (newHeight - _endJumpPos.y) / 2)
        {
            //_currentJumpDuration = _jumpDuration * Vector3.Distance(_startJumpPos, _endJumpPos) / _jumpLength;
            _currentJumpDuration = ((_jumpDuration /2) * (newHeight - _endJumpPos.y) / height) /2;
            Debug.Log("JUMP DURATION = "+_currentJumpDuration);
        }
            

        Keyframe[] keys = new Keyframe[3];
        keys[0] = new Keyframe(0f, _startJumpPos.y);
        keys[1] = new Keyframe(_jumpDuration / 2, newHeight);
        keys[2] = new Keyframe(_currentJumpDuration, _endJumpPos.y);

        //Tangentes pour parabole
        keys[0].inTangent = 0;
        keys[0].outTangent = (keys[1].value - keys[0].value) / (keys[1].time - keys[0].time);
        keys[1].inTangent = 0;
        keys[1].outTangent = 0;
        keys[2].inTangent = (keys[2].value - keys[1].value) / (keys[2].time - keys[1].time);
        keys[2].outTangent = 0;
        _jumpCurveY = new AnimationCurve(keys);
    }

    //Check de collision
    private bool IsJumpColliding(int resolution)
    {
        //Création de points de la courbe
        Vector3[] points = EvaluateJumpPositions(resolution);
        //Set de la normal d'atterissage à Vector3.up par défaut.
        //_endJumpNormal = Vector3.up;
        for (int i =1; i< points.Length; i++)
        {

            Vector3 direction = points[i] - points[i - 1];
            float distance = direction.magnitude;

            Debug.DrawRay(points[i - 1], direction, Color.green, 5f);
            RaycastHit hit;
            if (Physics.Raycast(points[i - 1], direction.normalized, out hit, distance, _wallMask))
            {
                //COLLIDING!
                _dJumpCollision.position = hit.point;
                //Debug.Log($"COLLIDING AT : {hit.point} ");
                _collisionJumpPos = hit.point;
                _endJumpNormal = hit.normal;

                ///TEST MULTIPLE POINTS
                Vector3[] squarePoints = Create4PointsFrom1PointAndNormal(points[i - 1], -hit.normal, .2f);
                RayCastFromPointsArray(squarePoints, -hit.normal, distance);

                //Debug.Log("JUMP NORMAL TRUE " + hit.normal);
                //Debug.Log("JUMP NORMAL TRUE " + _endJumpNormal);
                return true;

            }

        }
        //Debug.Log("NO COLLISION " + _endJumpNormal);
        return false;
    }

    private Vector3[] Create4PointsFrom1PointAndNormal(Vector3 origin, Vector3 normal, float squareSize)
    {
        Vector3[] points = new Vector3[4];
        Vector3 right = Vector3.Cross(Vector3.up, normal);
        Vector3 forward = Vector3.Cross(right, normal);
        
        points[0] = origin + (squareSize/2) * right + (squareSize/2) * forward;
        points[1] = origin + (squareSize/2) * -right + (squareSize/2) * forward;
        points[2] = origin + (squareSize/2) * -right + (squareSize/2) * -forward;
        points[3] = origin + (squareSize/2) * right + (squareSize/2) * -forward;

        return points;
    }

    private void RayCastFromPointsArray(Vector3[] squarePoints, Vector3 direction, float distance)
    {
        Vector3 newNormal = Vector3.zero;
        foreach (Vector3 point in squarePoints)
        {
            Debug.DrawRay(point, direction.normalized, Color.blue, 5f);
            RaycastHit hit;
            if (Physics.Raycast(point, direction.normalized, out hit, distance, _wallMask))
            {
                newNormal += hit.normal;
            }
            
        }
        if(newNormal != Vector3.zero)
            _endJumpNormal = (newNormal / 4).normalized;
        else
        {
            _endJumpNormal = direction;
        }
    }

    private Vector3[] EvaluateJumpPositions(int resolution)
    {
        Vector3[] points = new Vector3[resolution];

        for (int i = 0; i < resolution; i++)
        {
            float timer = i / (float)(resolution - 1);
            float jumpHeight = _jumpCurveY.Evaluate(timer * _currentJumpDuration);
            //Debug.Log("Evaluate jumpHeight" + jumpHeight);
            //TEST WITHOUT Y
            Vector3 startPos = new Vector3(_startJumpPos.x, 0, _startJumpPos.z);
            Vector3 endPos = new Vector3(_endJumpPos.x, 0, _endJumpPos.z);
            points[i] = Vector3.Lerp(startPos, endPos, timer) + Vector3.up * jumpHeight;
            //points[i] = Vector3.Lerp(_startJumpPos, _endJumpPos, timer) + Vector3.up * jumpHeight;
            _dTrajectory[i].position = points[i];
        }
        return points;
    }

    public void Jump(Vector2 moveValue)
    {
        if (!_isJumping)
        {
            //Trouver la destination
            FindJumpDestination(moveValue);
            SetStartJumpRotation();
            //Calculer la courbe
            SetJumpCurve(_currentHeight);

            //Gérer la collision
            if (!IsJumpColliding(10))
            {
                //Si pas de collision, on commence le saut
                SetEndJumpRotation();
                StartJumping();
            }
            else
            {
                //Si collision
                float tempHeight = _jumpHeight;
                
                //Debug.Log("JUMPING NORMAL " + _endJumpNormal);
                float collisionAngle = Vector3.Angle(_endJumpNormal, Vector3.up);
                Debug.Log("JUMPING COLLISION ANGLE = " + collisionAngle);
                //si surface plus ou moins vers le bas
                if (collisionAngle > 50)
                {
                    //On part vers le mur ====> SwitchToCrawlingMovement
                    _isGoingToWall = true;
                    tempHeight = _jumpHeight; 
                   
                }
                
                //On trouve la destination sur le mur.
                FindJumpDestinationCollision();
                //On set up la rotation à atteindre
                SetEndJumpRotationCollision();
                _currentHeight = tempHeight;

                //on set la courbe
                SetJumpCurve(_currentHeight);
                //on set la rotation de départ (on regarde vers là où on va)
                SetStartJumpRotation();
                StartJumping();
            }
        }

    }

    private void HandleJump()
    {
        if (_isJumping)
        {

            _jumpTimer += Time.deltaTime;
            float verticalMovement = _jumpCurveY.Evaluate(_jumpTimer);

            Vector2 startPos = new Vector2(_startJumpPos.x, _startJumpPos.z);
            Vector2 endPos = new Vector2(_endJumpPos.x, _endJumpPos.z);
            Vector2 horizontalMovement = Vector3.Lerp(startPos, endPos, _jumpTimer / _currentJumpDuration);

            if (_jumpTimer >= _currentJumpDuration)
            {
                _isJumping = false;
                _feetRig.weight = 1;
                if (_isGoingToWall && _characterMovement._isJumpingCreature)
                {

                    //Si on saute vers un mur, on switch le mouvement
                    _characterMovement.SwitchToCrawlingMovement();
                    _isGoingToWall = false;
                }

            }

            transform.position = new Vector3(horizontalMovement.x, verticalMovement, horizontalMovement.y);
            ManageJumpRotation();
        }
    }

    #endregion

   
    private void HandleMoveJumping(Vector2 moveValue)
    {
        if (_characterMovement.CheckMoving())
        {
            Jump(moveValue);
        }
    }

  


    
}

