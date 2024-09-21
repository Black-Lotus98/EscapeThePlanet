// This script will replace the old Collectable scripts and implement the strategy pattern
// This is a parent class that the Collectable scripts will inherit from such as the FuelBarrel and ShieldPowerUp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable<T> : MonoBehaviour where T : UIManager
{
    [SerializeField] protected ParticleSystem ExplosionEffect;

    // This is a part of the strategy pattern, it will be used to determine the type of collectable
    protected ICollectibleBehavior<T> collectibleBehavior;

    // This is the default behavior of the collectable
    private void OnTriggerEnter(Collider other)
    {
        var manager = other.GetComponent<T>();

        if (manager != null)
        {
            Collect(manager);

            Instantiate(ExplosionEffect, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
            Destroy(gameObject);
        }
    }

    protected abstract void Collect(T manager);
}

