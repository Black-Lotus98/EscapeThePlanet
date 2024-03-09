using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollectible : ICollectibleBehavior<KeyManager>
{
    private bool hasKey;

    public KeyCollectible(bool hasKey)
    {
        this.hasKey = hasKey;
    }

    public void ExecutePowerUp(KeyManager keyManager)
    {
        keyManager.PlayerHasKey = hasKey;
    }

    // public void ExecutePowerUp(Player player)
    // {
    //     // Debug.Log($"Execute key power up");
    //     player.SetKeyStatus(hasKey);
    // }
}
