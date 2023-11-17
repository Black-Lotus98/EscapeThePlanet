using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "FuelPad State/ActiveFuelPadState")]
public abstract class FuelPadState : ScriptableObject
{
    public float refillCooldown;
    public float RefillSpeed = 1;
    public bool isPlayerInside = false;

    public abstract void Enter(FuelPad pad);
    public abstract void Exit(FuelPad pad);
    public abstract void UpdateStats(FuelPad pad);
}
