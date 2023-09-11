using UnityEngine;
public class FuelBarrel : Collectable
{

    [SerializeField] int amount;
    private void Start()
    {
        collectibleBehavior = new FuelBarrelCollectible(amount);
    }

    protected override void Collect(Player player)
    {
        collectibleBehavior.ExecutePowerUp(player);
    }
}
