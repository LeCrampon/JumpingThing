using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraTransitionHelper : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private CharacterMovement[] _characterMovements;
    public int _characterIndex = 0;
    [SerializeField]
    private Transform _menuPosition;
    [SerializeField]
    private CharacterInput _characterInput;
    [SerializeField]
    private GrassTrample _grassTrample;

    [Header("Who am i following?")]
    [SerializeField]
    private CharacterMovement _currentlyFollowing = null;
    [SerializeField]
    private CharacterMovement _nextFollowing = null;
    [SerializeField]
    private CharacterMovement _previousFollowing = null;

    [Header("Lerp values")]
    [SerializeField]
    private float _lerpSpeed = 1f;
    [SerializeField]
    private float _maxTime = 1.5f;
    [SerializeField]
    private float _timer = 5f;

    public void SwitchToNextCharacter(CharacterMovement _nextCharacter)
    {
        if(_currentlyFollowing == _nextCharacter)
        {
            return;
        }
        GameStateManager._instance.SwitchCharacterMusic(_nextFollowing, _nextCharacter);
        _previousFollowing = _nextFollowing;
        _nextFollowing = _nextCharacter;
        GameStateManager._instance.SetCurrentCharacter(_nextCharacter);


    }

    private void StartTransition()
    {
        _timer = 0f;
        GameStateManager._instance._isInTransition = true;
        //Parent To Next Character
        ParentToNextCharacter();
    }

    private void ParentToNextCharacter()
    {
        //RETIRER CAMERA DE LA DETECTION d'OBSTACLES
        if(_previousFollowing != null && _previousFollowing != _nextFollowing)
        {
            Debug.Log("REMOVING PREVIOUS FOLLOWING");
            _previousFollowing._cameraControls.GetComponent<CameraObstacleDetection>()._cameraTransform = null;

        }

        _nextFollowing._mainCamera = _camera;
        _nextFollowing._jumpingMovement._mainCamera = _camera;
        _nextFollowing._crawlingMovement._bodyIK._camera = _camera;
        _nextFollowing._cameraControls.GetComponent<CameraObstacleDetection>()._cameraTransform = _camera.transform;

        //_nextFollowing.transform.parent.gameObject.SetActive(true);
        _camera.transform.SetParent(_nextFollowing._cameraTarget, true);
        _characterInput.SwitchCharacterMovement(_nextFollowing);
        _grassTrample.transform.SetParent(_nextFollowing.transform);
        _grassTrample.transform.localPosition = Vector3.zero;
        _grassTrample.transform.localRotation = Quaternion.identity;

        //PostProcess
        GameStateManager._instance.GetPostProcessManagement().SwitchPostProcessValues(GameStateManager._instance.GetCurrentCharacter());
        GameStateManager._instance.GetPostProcessManagement().SetCamera(_nextFollowing._cameraControls.GetComponent<CameraObstacleDetection>());
        //if(_currentlyFollowing != null)
        //    _currentlyFollowing.transform.parent.gameObject.SetActive(false);

    }

    private void TransitionToNextCharacter()
    {
        //Vector3 startPos = Vector3.zero;
        if(_timer < _maxTime)
        {

            //if(_timer == 0)
            //{
            //    startPos = _camera.transform.localPosition; 
            //}


            _timer += Time.deltaTime;

            //Determine finalPosition
            Vector3 finalPosition = Vector3.zero;
            float lerpValue = _timer / _maxTime;
            //Debug.Log("LERPVALUE " + lerpValue);


            //lerp Position to final Position
            _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, finalPosition, _timer / _maxTime * _lerpSpeed);
            _camera.transform.localRotation = Quaternion.Slerp(_camera.transform.localRotation, Quaternion.identity, _timer / _maxTime * _lerpSpeed);
            //Debug.Log("============== IN TRANSITION ==============");
            //Debug.Log("CurrentPos : " + _camera.transform.localPosition);
            //Debug.Log(_timer / _maxTime * _lerpSpeed);
            //Debug.Log("============== IN TRANSITION ==============");
            return;
        }
        else
        {
            GameStateManager._instance._isInTransition = false;
        }

        if (_currentlyFollowing != _nextFollowing)
            _currentlyFollowing = _nextFollowing;
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Keypad7))
        //{
        //    SwitchToNextCharacter(_crawlingBugMovement);
        //    StartTransition();
        //}

        //if (Input.GetKeyDown(KeyCode.Keypad8))
        //{
        //    SwitchToNextCharacter( _jumpingBugMovement);
        //    StartTransition();
        //}

        //if (Input.GetKeyDown(KeyCode.Keypad9))
        //{
        //    SwitchToNextCharacter( _flyingBugMovement);
        //    StartTransition();
        //}

        TransitionToNextCharacter();
    }

    public void GoToNextCharacter()
    {
        int nextIndex = ((_characterIndex + 1) % _characterMovements.Length + _characterMovements.Length) % _characterMovements.Length;
        Debug.Log("next index " + nextIndex);
        SwitchToNextCharacter(_characterMovements[nextIndex]);
        StartTransition();
        _characterIndex = nextIndex;
    }

    public void GoToCharacterByIndex(int index)
    {
        SwitchToNextCharacter(_characterMovements[index]);
        StartTransition();
        _characterIndex = index;    
    }

    public void GoToPreviousCharacter()
    {
        int previousIndex = ((_characterIndex - 1) % _characterMovements.Length + _characterMovements.Length) % _characterMovements.Length;
        Debug.Log("previous index " + previousIndex);
        SwitchToNextCharacter(_characterMovements[previousIndex]);
        SwitchToNextCharacter(_characterMovements[previousIndex]);
        StartTransition();
        _characterIndex = previousIndex;
    }
}
