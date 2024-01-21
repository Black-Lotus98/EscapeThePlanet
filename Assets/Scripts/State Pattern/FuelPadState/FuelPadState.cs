using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FuelPadState
{
    public abstract void ActivateState(FuelPad pad);
    public abstract void DeactivateState(FuelPad pad);
}
