using UnityEngine;

public class Key : Collectable<KeyManager>
{
    private void Start()
    {
        collectibleBehavior = new KeyCollectible(true);
    }
    protected override void Collect(KeyManager keyManager)
    {
        collectibleBehavior.ExecutePowerUp(keyManager);
    }
}
