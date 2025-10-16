using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShroomPoisoning : MonoBehaviour
{
    private PostProcessManagement _postProcess;
    [SerializeField]
    private CharacterMovement _characterMovement;
   
    // Start is called before the first frame update
    void Start()
    {
        _postProcess = GameStateManager._instance.GetPostProcess();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            //_postProcess.StartPoisoning();
            _characterMovement._isPoisoned = true;
            
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            //_postProcess.StopPoisoning();
            _characterMovement._isPoisoned = false;
            //GameStateManager._instance.SwitchMusicFromPoisoned(_characterMovement);
        }

    }
}
