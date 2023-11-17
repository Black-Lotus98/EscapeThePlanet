using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// The Collision observer interface
public interface ICollisionObserver
{
    void OnStart();
    void OnFinish();
    void OnFuelPad();
    void OnCrash();
}
