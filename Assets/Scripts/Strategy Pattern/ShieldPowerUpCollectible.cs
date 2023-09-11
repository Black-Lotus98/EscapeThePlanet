// this is a concrete class that implements the ICollectibleBehavior
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUpCollectible : ICollectibleBehavior

{
    private int amount;

    public ShieldPowerUpCollectible(int amount)
    {
        this.amount = amount;
    }

    public void ExecutePowerUp(Player player)
    {
        player.IncreaseShieldTime(5.0f);
    }
}
