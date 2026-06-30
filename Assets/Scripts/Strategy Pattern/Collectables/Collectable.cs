// This script will replace the old Collectable scripts and implement the strategy pattern
// This is a parent class that the Collectable scripts will inherit from such as the FuelBarrel and ShieldPowerUp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable<T> : MonoBehaviour, ICheckpointable where T : UIManager
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

            ParticleSystem effect = Instantiate(ExplosionEffect, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
            var main = effect.main;
            Destroy(effect.gameObject, main.duration + main.startLifetime.constantMax);

            // Deactivate instead of Destroy so a checkpoint respawn can bring the
            // collectible back if it was collected after the checkpoint was reached.
            gameObject.SetActive(false);
        }
    }

    protected abstract void Collect(T manager);

    // Memento Pattern: a collectible's only checkpointed state is whether it is still
    // present (active) in the world at the moment the checkpoint snapshot was taken.
    public virtual object CaptureState()
    {
        return gameObject.activeSelf;
    }

    public virtual void RestoreState(object memento)
    {
        if (memento is bool active)
        {
            gameObject.SetActive(active);
        }
    }
}

