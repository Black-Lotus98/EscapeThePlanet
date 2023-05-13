using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUpStrategy : IPowerUpStrategy
{
    public void ExecutePowerUp(Player player)
    {
        player.IncreaseShieldTime(5.0f);
    }
}
