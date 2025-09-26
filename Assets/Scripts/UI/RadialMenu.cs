using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadialMenu : MonoBehaviour
{
    [SerializeField]
    private RadialItem[] _radialItems;
    [SerializeField]
    private int _highlightedIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SelectCharacter();
    }

    private void SelectCharacter()
    {
        Debug.Log("Mouse Angle " + GetMouseAngle());
        float mouseAngle = GetMouseAngle();

        if (AngleIsBetween(mouseAngle, -60f, 60f))
        {
            HighlightCurrentCharacter(0);
        }
        else if (AngleIsBetween(mouseAngle, 60, 180f))
        {
            HighlightCurrentCharacter(1);
        }
        else if (AngleIsBetween(mouseAngle, -180f, -60f))
        {
            HighlightCurrentCharacter(2);
        }
    }

    private void HighlightCurrentCharacter(int index)
    { 
        for (int i= 0; i< _radialItems.Length; i++)
        {
            if(i == index)
            {
                _radialItems[i].SetHighlighted(true);
                _highlightedIndex = i;
            }
            else
            {
                _radialItems[i].SetHighlighted(false);
            }
                
        }
    }

    public int GetHighlightedIndex()
    {
        return _highlightedIndex;
    }

    private bool AngleIsBetween(float angle, float min, float max)
    {
        return angle > min && angle <= max;
    }

    private Vector2 GetCenterPosition()
    {
        return new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    private Vector2 GetMousePosition()
    {
        return Input.mousePosition;
    }

    private float GetMouseAngle()
    {
        Vector2 mouseDir;
        if(GameStateManager._instance._input.currentControlScheme == "Gamepad")
        {
            mouseDir = GetStickPosition();
        }
        else
        {
            mouseDir = GetMousePosition() - GetCenterPosition();
        }
            
        Vector2 upDir = Vector2.up;

        return Vector2.SignedAngle(upDir, mouseDir);
    }

    private Vector2 GetStickPosition()
    {
        return GameStateManager._instance.GetCharacterInput().GetLookValue();
    }
}
