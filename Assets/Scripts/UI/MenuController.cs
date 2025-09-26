using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject _firstSelected;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(_firstSelected);
    }
}
