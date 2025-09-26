using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ContextualUI : MonoBehaviour
{
    [SerializeField]
    private Image _keyIcon;
    [SerializeField]
    private TMP_Text _contextualText;
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private ContextualUIData _switchKeyboard, _jumpOffKeyboard, _flyKeyboard;
    [SerializeField]
    private ContextualUIData _switchGamepad, _jumpOffGamepad, _flyGamepad;

    private bool _isSwitching = false;

    public void StartSession()
    {
        StartCoroutine(FadeIn());
    }

    public void PauseSession()
    {
        StartCoroutine(FadeOut());
    }

    private void LoadData(ContextualUIData data)
    {
        _keyIcon.sprite = data.keyIcon;
        _contextualText.text = data.keyText;
    }

    private IEnumerator FadeOut()
    {

        while (_canvasGroup.alpha > 0f)
        {
            yield return new WaitForEndOfFrame();
            _canvasGroup.alpha -= .05f;
        }

    }

    private IEnumerator FadeIn()
    {

        while(_canvasGroup.alpha < 1f)
        {
            yield return new WaitForEndOfFrame();
            _canvasGroup.alpha += .05f;
        }

    }

    private IEnumerator SwitchUIElement(ContextualUIData data)
    {
        if(_contextualText.text != data.keyText)
        {
            _isSwitching = true;
            yield return StartCoroutine(FadeOut());
            LoadData(data);
            StartCoroutine(FadeIn());
            _isSwitching = false;
        }
    
    }

    public void SwitchUIElementToSwitch()
    {
        if(GameStateManager._instance._input.currentControlScheme == "Gamepad")
        {
            StartCoroutine(SwitchUIElement(_switchGamepad));
            return;
        }
        StartCoroutine(SwitchUIElement(_switchKeyboard));
    }

    public void SwitchUIElementToFly()
    {
        if (GameStateManager._instance._input.currentControlScheme == "Gamepad")
        {
            StartCoroutine(SwitchUIElement(_flyGamepad));
            return;
        }
        StartCoroutine(SwitchUIElement(_flyKeyboard));
    }

    public void SwitchUIElementToJumpOff()
    {
        if (GameStateManager._instance._input.currentControlScheme == "Gamepad")
        {
            StartCoroutine(SwitchUIElement(_jumpOffGamepad));
            return;
        }
        StartCoroutine(SwitchUIElement(_jumpOffKeyboard));
    }

    private void Update()
    {
        if (GameStateManager._instance._started && GameStateManager._instance.GetCurrentCharacter() != null && !_isSwitching)
        {
            if (IsACrawlingWasp())
            {
                SwitchUIElementToFly();
                return;
            }

            if (IsAFlyingWasp() || IsAJumpingJumper() || IsACrawler())
            {
                SwitchUIElementToSwitch();
                return;
            }

            if (IsACrawlingJumper())
            {
                SwitchUIElementToJumpOff();
                return;
            }
        }
        
    }

    private bool IsAFlyingWasp()
    {
        return GameStateManager._instance.GetCurrentCharacter()._isFlyingCreature && GameStateManager._instance.GetCurrentMovementType() == MovementType.FlyingMovement;
    }

    private bool IsACrawlingWasp()
    {
        return GameStateManager._instance.GetCurrentCharacter()._isFlyingCreature && GameStateManager._instance.GetCurrentMovementType() == MovementType.CrawlingMovement;
    }

    private bool IsACrawlingJumper()
    {
        return GameStateManager._instance.GetCurrentCharacter()._isJumpingCreature && GameStateManager._instance.GetCurrentMovementType() == MovementType.CrawlingMovement;
    }

    private bool IsAJumpingJumper()
    {
        return GameStateManager._instance.GetCurrentCharacter()._isJumpingCreature && GameStateManager._instance.GetCurrentMovementType() == MovementType.JumpingMovement ;
    }

    private bool IsACrawler()
    {
        return !GameStateManager._instance.GetCurrentCharacter()._isJumpingCreature && !GameStateManager._instance.GetCurrentCharacter()._isFlyingCreature;
    }

}
