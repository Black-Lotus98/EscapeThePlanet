using UnityEngine;

public class ShieldPowerUp : Collectable<ShieldManager>
{
    [SerializeField] int amount;
    private void Start()
    {
        collectibleBehavior = new ShieldPowerUpCollectible(amount);
    }

    protected override void Collect(ShieldManager shieldManager)
    {
        collectibleBehavior.ExecutePowerUp(shieldManager);
    }
}
