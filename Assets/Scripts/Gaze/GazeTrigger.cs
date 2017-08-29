using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class GazeTrigger : MonoBehaviour
{
    public bool trackProgress = true;

    private void OnGazeEnter()
    {
        //if (!OVRPlugin.userPresent || !GazeManager.canGaze) return;
        if (!GazeManager.canGaze) return;

        Debug.Log("OnGazeEnter()");
        
        GazeManager.Instance.GazeTo(this.gameObject, trackProgress);
    }

    private void OnGazeLeave()
    {
        //if (!OVRPlugin.userPresent || !GazeManager.canGaze) return;
        if (!GazeManager.canGaze) return;

        Debug.Log("OnGazeLeave()");
        GazeManager.Instance.GazeOff();
    }

    private void OnGazeComplete()
    {
        
    }

}
