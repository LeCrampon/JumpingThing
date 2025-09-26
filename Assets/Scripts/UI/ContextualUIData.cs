using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ContextualUIData", order = 2)]
public class ContextualUIData : ScriptableObject
{
    public Sprite keyIcon;
    public string keyText;
}
