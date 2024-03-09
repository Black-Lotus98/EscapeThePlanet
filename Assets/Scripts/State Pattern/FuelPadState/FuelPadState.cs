using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FuelPadState
{
    // Sample code to set the amount of fuel in the fuel pad
    // public void SetFuelAmount(FuelPad pad, int amount)
    // {
    //     pad.refillAmount = amount;
    // }

    public abstract void ActivateState(FuelPad pad);
    public abstract void DeactivateState(FuelPad pad);
}
