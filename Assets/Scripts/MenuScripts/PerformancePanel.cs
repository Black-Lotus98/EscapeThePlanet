using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script must be attached to the performance panel
public class PerformancePanel : MonoBehaviour
{
    public LoadStatus loadStatus;

    private void OnEnable()
    {
        loadStatus.LoadProgress();
    }

}
