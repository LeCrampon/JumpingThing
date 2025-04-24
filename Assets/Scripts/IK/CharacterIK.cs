using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Foot_IK[] _feet;
    [SerializeField]
    private Foot_IK[] _feetSetFLBR;
    [SerializeField]
    private Foot_IK[] _feetSetFRBL;


    [Header("Feet Values")]
    public float _stepSpeed;
    public float _stepSize;
    public float _stepHeight;
    public LayerMask _groundMask;
    public GroundChecker _groundChecker;

    [SerializeField]
    private float _movementSpeed;

    [SerializeField]
    private bool _alternance;

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

        TEST_NoCycleLegs();
    

    }

    private void TEST_CycleLegs()
    {
        if (_alternance)
        {
            int count = 0;
            foreach(Foot_IK foot in _feetSetFLBR)
            {
                foot.ManageFootMovement();
                if (foot._isMoving)
                {
                    count++;
                }
            }
            if (count == 0)
                _alternance = false;
        }
        else
        {
            int count = 0;
            foreach (Foot_IK foot in _feetSetFRBL)
            {
                foot.ManageFootMovement();
                if (foot._isMoving)
                {
                    count++;
                }
            }
            if (count == 0)
                _alternance = true;
        }
    }

    private void TEST_NoCycleLegs()
    {
        foreach (Foot_IK foot in _feetSetFRBL)
        {
            foot.ManageFootMovement();
        }

        foreach (Foot_IK foot in _feetSetFLBR)
        {
            foot.ManageFootMovement();
        }

    }

 




}
