using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script must be attached to the performance panel
public class PerformancePanel : MonoBehaviour
{
    public LoadStatus loadStatus;

    private void OnEnable()
    {
        if (loadStatus == null)
        {
            Debug.LogError("PerformancePanel: loadStatus is not assigned!", this);
            return;
        }
        loadStatus.LoadProgress();
    }

}
