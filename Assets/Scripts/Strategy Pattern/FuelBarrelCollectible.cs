// this is a concrete class that implements the ICollectibleBehavior
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelBarrelCollectible : ICollectibleBehavior<FuelManager>
{
    private int amount;

    public FuelBarrelCollectible(int amount)
    {
        this.amount = amount;
    }
    
    public void ExecutePowerUp(FuelManager fuelManager)
    {
        Debug.Log($"fuelManager.FuelCounter " + fuelManager.FuelAmount);
        if (fuelManager.FuelAmount < fuelManager.MaxFlightTime)
        {
            fuelManager.FuelBarrel(amount);
        }
    }
    
    // public void ExecutePowerUp(Player player)
    // {
    //     if (player.GetFuelCounter() < player.GetMaxFlightTime())
    //     {
    //         player.FuelBarrel(amount);
    //     }
    // }
}