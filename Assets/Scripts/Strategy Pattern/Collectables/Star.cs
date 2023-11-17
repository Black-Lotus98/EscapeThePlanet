using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Collectable<StarsManager>
{

    [SerializeField]  int amount;
    private void Start()
    {
        collectibleBehavior = new StarCollectible(amount);
    }

    protected override void Collect(StarsManager starsManager)
    {
        collectibleBehavior.ExecutePowerUp(starsManager);
    }
}
