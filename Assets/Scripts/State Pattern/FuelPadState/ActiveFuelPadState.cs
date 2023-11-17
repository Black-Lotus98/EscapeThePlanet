using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveFuelPadState : FuelPadState
{

    public override void Enter(FuelPad pad)
    {
        this.isPlayerInside = pad.isPlayerInside;
        Debug.Log($"{isPlayerInside} from inside of the fuel pad state");
    }

    public override void Exit(FuelPad pad)
    {
        this.isPlayerInside = pad.isPlayerInside;
        Debug.Log($"{isPlayerInside} from outside of the fuel pad state");


    }

    public override void UpdateStats(FuelPad pad)
    {
        if (this.isPlayerInside)
        {
            if (refillCooldown > 0f)
            {
                if (pad.player.GetFuelCounter() < 60)
                {
                    pad.player.FuelUsage(RefillSpeed);
                    refillCooldown -= RefillSpeed * Time.deltaTime;
                }
                else if (pad.player.GetFuelCounter() > 60)
                {
                    pad.AS.Stop();
                    // player.SetFuelCounter(60);
                }
            }
            else
            {
                pad.AS.Stop();
                refillCooldown = 0;
            }
            if ((!pad.player.enabled) && (pad.player.GetFuelCounter() > 0))
            {
                pad.player.enabled = true;
            }

        }
        else
        {
            return;
        }
    }
}
