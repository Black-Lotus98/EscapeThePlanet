using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPadCollisionState : MonoBehaviour, CollisionState
{
    public void Handle(CollisionHandler context)
    {
        Debug.Log("You hit the fuel pad, FuelPadState");
    }
}
