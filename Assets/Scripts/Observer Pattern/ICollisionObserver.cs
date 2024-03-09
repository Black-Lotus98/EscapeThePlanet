using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Currently not in use

// The Collision observer interface
public interface ICollisionObserver
{
    void OnStart();
    void OnFinish();
    void OnFuelPad();
    void OnCrash();
}
