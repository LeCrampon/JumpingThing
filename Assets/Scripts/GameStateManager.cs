using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager _instance;

    [SerializeField]
    private float _timeScale = 1f;
    [SerializeField]
    [Range(0.1f,2f)]
    private float _mouseSensitivity = 1f;

    [Header("References")]
    [SerializeField]
    private CharacterInput _characterInput;
    [SerializeField]
    public CameraTransitionHelper _cameraTransitionHelper;
    [SerializeField]
    private CharacterMovement _currentCharacter;
    [SerializeField]
    private PostProcessManagement _postProcess;
    [SerializeField]
    private MusicManager _musicManager;
    [SerializeField]
    public PlayerInput _input;
    [SerializeField]
    private Camera _mainCamera;

    [Header("Bools")]
    private bool _paused;
    [SerializeField]
    public bool _isInMenu = true;
    [SerializeField]
    public bool _isInTransition = false;
    [SerializeField]
    public bool _started = false;

    [Header("UI Elements")]
    [SerializeField]
    private MainMenu _mainMenu;
    [SerializeField]
    private RadialMenu _radialMenu;
    [SerializeField]
    private GameObject _pauseMenu;
    [SerializeField]
    private GameObject _settingsMenu;
    [SerializeField]
    private ContextualUI _contextualUI;

    [Header("Settings Sliders")]
    [SerializeField]
    private Slider _sensitivitySlider;
    [SerializeField]
    private Slider _fxVolumeSlider;
    [SerializeField]
    private Slider _musicVolumeSlider;

    [Header("Audio Mixers")]
    [SerializeField]
    private AudioMixer _fxMixer;
    [SerializeField]
    private AudioMixer _musicMixer;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    public Camera GetMainCamera()
    {
        return _mainCamera;
    }

    public MainMenu GetMainMenu()
    {
        return _mainMenu;
    }
    private void Start()
    {
        SetSliders();
    }

    public CharacterInput GetCharacterInput()
    {
        return _characterInput;
    }

    public void OpenSettingsMenu()
    {
        _pauseMenu.SetActive(false);
        _settingsMenu.SetActive(true);
    }

    public void BackToPauseMenu()
    {
        _settingsMenu.SetActive(false);
        _pauseMenu.SetActive(true);
    }

   
    public void ActivateLowPass()
    {
        _fxMixer.SetFloat("LowPassMixValue", 0f);
        _musicMixer.SetFloat("LowPassMixValue", 0f);
    }

    public void DeActivateLowPass()
    {
        _fxMixer.SetFloat("LowPassMixValue", -80f);
        _musicMixer.SetFloat("LowPassMixValue", -80f);
    }

    public void SetMouseSensitivity()
    {
        _mouseSensitivity = _sensitivitySlider.value;
        Debug.Log("Sensitivity " + _mouseSensitivity);
    }

    public void SetFxVolume()
    {
        _fxMixer.SetFloat("MasterVolume", Mathf.Log10(_fxVolumeSlider.value) * 20);
    }

    public void SetMusicVolume()
    {
        _musicMixer.SetFloat("MasterVolume", Mathf.Log10(_musicVolumeSlider.value) * 20);
    }

    public float GetMouseSensitivity()
    {
        return _mouseSensitivity;
    }

    public void SetSliderSensitivity()
    {
        _sensitivitySlider.value = _mouseSensitivity;
    }

    public void SetSliderMusicVolume()
    {
        float value = 1f;
        _musicMixer.GetFloat("MasterVolume", out value);
        _musicVolumeSlider.value = Mathf.Pow(10, value / 20f);
    }

    public void SetSliderFxVolume()
    {
        float value = 1f;
        _fxMixer.GetFloat("MasterVolume", out value);
        _fxVolumeSlider.value = Mathf.Pow(10, value / 20f);
    }

    public void SetSliders()
    {
        SetSliderSensitivity();
        SetSliderMusicVolume();
        SetSliderFxVolume();
    }

    public PostProcessManagement GetPostProcessManagement()
    {
        return _postProcess;
    }


    public void PauseGame()
    {
        if (_paused)
        {
            return;
        }
        _paused = true;
        _timeScale = 0f;
        _contextualUI.PauseSession();
        _pauseMenu.SetActive(true);
        SwitchCurrentActionMap("UI");
        ActivateLowPass();
    }

    public void UnPauseGame()
    {
        if(!_paused)
        {
            return;
        }

        _paused = false;
        _timeScale = 1f;
        _contextualUI.StartSession();
        _pauseMenu.SetActive(false);
        SwitchCurrentActionMap("Player");
        DeActivateLowPass();
    }
    
    public void SwitchCurrentActionMap(string actionMap)
    {
        _characterInput.SwitchActionMap(actionMap);
    }

    public void SetCurrentCharacter(CharacterMovement characterMovement)
    {
        _currentCharacter = characterMovement;
    }

    public CharacterMovement GetCurrentCharacter()
    {
        return _currentCharacter;
    }

    public MovementType GetCurrentMovementType()
    {
        return _currentCharacter._movementType;
    }

    public void OpenRadialMenu()
    {
        //if (_radialMenu.gameObject.activeInHierarchy)
        //    SwitchCurrentActionMap("Player");
        //else
        //    SwitchCurrentActionMap("UI");
        _radialMenu.gameObject.SetActive(true);
        _timeScale = .2f;
    }

    public void SwitchCharacterMusic(CharacterMovement from, CharacterMovement to)
    {
        _musicManager.SwitchCharacterTrack(from, to);
    }

    public void CloseRadialMenu()
    {
        _cameraTransitionHelper.GoToCharacterByIndex(_radialMenu.GetHighlightedIndex());
        //_cameraTransitionHelper.GoToNextCharacter();
        _radialMenu.gameObject.SetActive(false);
        _timeScale = 1f;
    }


    private void Update()
    {

        DEBUG_ChangeTimeScale();
    }

    private void DEBUG_ChangeTimeScale()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            _timeScale += .1f;
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            _timeScale -= .1f;
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            _timeScale = 0;
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            _timeScale = 1;
        }


        Time.timeScale = _timeScale;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
