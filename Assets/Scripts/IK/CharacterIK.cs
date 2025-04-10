using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    [SerializeField]
    private Foot_IK[] _feet;

    public float _stepSpeed;
    public float _stepSize;
    public float _stepHeight;
    public LayerMask _groundMask;
    public GroundChecker _groundChecker;

    [SerializeField]
    private float _movementSpeed;


    public bool IsAnotherFootDown(Foot_IK currentFoot)
    {
        foreach(Foot_IK foot in _feet)
        {
            if (foot == currentFoot)
                break;
            if (foot._isFootDown)
                return true;
        }

        return false;
    }

    public void DEBUG_MoveInFront()
    {
        transform.position += transform.forward * Time.deltaTime * _movementSpeed;
    }

    private void Update()
    {
        DEBUG_MoveInFront();
    }

    //private void MoveEachLeg()
    //{
    //    foreach( Foot_IK foot in _feet)
    //    {
    //        if(is)
    //    }
    //}
}
