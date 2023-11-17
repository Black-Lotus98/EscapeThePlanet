using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUp : Command
{
    private float movementSpeed;
    private AudioClip mainEngine;
    private ParticleSystem rocketBoostParticles;

    private FuelManager fuelManager;

    public MoveUp(float movementSpeed, AudioClip mainEngine, ParticleSystem rocketBoostParticles, FuelManager fuelManager)
    {
        this.movementSpeed = movementSpeed;
        this.mainEngine = mainEngine;
        this.rocketBoostParticles = rocketBoostParticles;
        this.fuelManager = fuelManager;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        if (fuelManager.IsUsingFuel)
        {
            if (fuelManager.FuelAmount <= 0)
            {
                fuelManager.FuelAmount = 0;
                audioSource.Stop();
                return;
            }
            fuelManager.FuelConsumption(-1);
            fuelManager.NotifyObservers(UIState.FuelChanged);
        }
        rigidbody.AddRelativeForce(Vector3.up * Time.deltaTime * movementSpeed);

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        if (!rocketBoostParticles.isPlaying)
        {
            rocketBoostParticles.Play();
        }
    }
}
