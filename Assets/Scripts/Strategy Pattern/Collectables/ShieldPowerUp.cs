using UnityEngine;

public class ShieldPowerUp : Collectable
{
    [SerializeField] int amount;
    private void Start()
    {
        collectibleBehavior = new ShieldPowerUpCollectible(amount);
    }

    protected override void Collect(Player player)
    {
        collectibleBehavior.ExecutePowerUp(player);
    }
}
