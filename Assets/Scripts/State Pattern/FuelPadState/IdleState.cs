using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdleState : FuelPadState
{

    public override void ActivateState(FuelPad pad)
    {
        Debug.Log("Idle FuelPad State: Activated");

    }
    public override void DeactivateState(FuelPad pad)
    {
        Debug.Log("Idle FuelPad State: Deactivated");
    }
}
