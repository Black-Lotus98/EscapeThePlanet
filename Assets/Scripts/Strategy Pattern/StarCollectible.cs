using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarCollectible : ICollectibleBehavior<StarsManager>
{
    private int starCount;

    public StarCollectible(int amount)
    {
        starCount = amount;
    }

    public void ExecutePowerUp(StarsManager starsManager)
    {
        // Debug.Log($"Star Collected");
        starsManager.CollectedStarsCounter = starCount;
    }
}
