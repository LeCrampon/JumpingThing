using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _startMenu;
    [SerializeField]
    private GameObject _characterSelectMenu;
  

    public void StartGame()
    {
        GameStateManager._instance._isInMenu = false;
        gameObject.SetActive(false);
        GameStateManager._instance.SwitchCurrentActionMap("Player");
    }

    public void GoToCharacterSelect()
    {
        _startMenu.SetActive(false);
        GameStateManager._instance._cameraTransitionHelper.GoToCharacterByIndex(0);
        _characterSelectMenu.SetActive(true);
    }

    public void GoToStartMenu()
    {
        _characterSelectMenu.SetActive(false);
        _startMenu.SetActive(true);
    }

    public void GoLeftCharacter()
    {
        GameStateManager._instance._cameraTransitionHelper.GoToPreviousCharacter();
    }

    public void GoRightCharacter()
    {
        GameStateManager._instance._cameraTransitionHelper.GoToNextCharacter();
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
