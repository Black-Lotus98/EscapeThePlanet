using UnityEngine;
public class FuelBarrel : Collectable<FuelManager>
{
    [SerializeField] int amount;
    private void Start()
    {
        collectibleBehavior = new FuelBarrelCollectible(amount);
    }
    protected override void Collect(FuelManager fuelManager)
    {
        collectibleBehavior.ExecutePowerUp(fuelManager);
    }

    
    // protected override void Collect(Player player)
    // {
    //     collectibleBehavior.ExecutePowerUp(player);
    // }
}
