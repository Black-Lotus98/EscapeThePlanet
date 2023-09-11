// this is a concrete class that implements the ICollectibleBehavior
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelBarrelCollectible : ICollectibleBehavior
{
    private int amount;

    public FuelBarrelCollectible(int amount)
    {
        this.amount = amount;
    }

    public void ExecutePowerUp(Player player)
    {
        if (player.GetFuelCounter() < player.GetMaxFlightTime())
        {
            player.FuelBarrel(amount);
        }
    }
}