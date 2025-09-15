using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PostProcessData", order = 1)]
public class PostProcessData : ScriptableObject
{

  
    public float minDistance = .2f;

    public float maxDistance = 3.5f;


    public float minFocal = .1f;

    public float maxFocal = .8f;
}
