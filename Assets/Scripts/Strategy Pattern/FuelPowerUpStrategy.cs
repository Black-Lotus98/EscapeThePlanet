using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPowerUpStrategy : IPowerUpStrategy
{
    public void ExecutePowerUp(Player player)
    {
        player.FuelBarrel(5);
    }
}
