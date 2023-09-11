// This script will replace the old Collectable scripts and implement the strategy pattern
// This is a parent class that the Collectable scripts will inherit from such as the FuelBarrel and ShieldPowerUp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    [SerializeField] protected ParticleSystem ExplosionEffect;

    // This is a part of the strategy pattern, it will be used to determine the type of collectable
    protected ICollectibleBehavior collectibleBehavior;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();

        if (player != null)
        {
            Collect(player);

            Instantiate(ExplosionEffect, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
            Destroy(gameObject);
        }
    }

    protected abstract void Collect(Player player);
}

