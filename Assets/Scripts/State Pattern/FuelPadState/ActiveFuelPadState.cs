using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActiveFuelPadState : FuelPadState
{
    public override void ActivateState(FuelPad pad)
    {
        // Sample code to set fuel amount
        // SetFuelAmount(pad, 100);
        pad.StartFuelRefill();
        Debug.Log("Active FuelPad State: Activated");
    }

    public override void DeactivateState(FuelPad pad)
    {
        pad.StopFuelRefill();
        Debug.Log("Active FuelPad State: Deactivated");
    }
}

