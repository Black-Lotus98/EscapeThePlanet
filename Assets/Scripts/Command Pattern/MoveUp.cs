using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUp : Command
{
    private readonly float movementSpeed;
    private readonly AudioClip mainEngine;
    private readonly ParticleSystem rocketBoostParticles;
    private readonly FuelManager fuelManager;

    public MoveUp(float movementSpeed, AudioClip mainEngine, ParticleSystem rocketBoostParticles, FuelManager fuelManager)
    {
        this.movementSpeed = movementSpeed;
        this.mainEngine = mainEngine;
        this.rocketBoostParticles = rocketBoostParticles;
        this.fuelManager = fuelManager;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        // Check fuel consumption first
        if (!HandleFuelConsumption(audioSource))
        {
            return; // Exit if no fuel
        }
        
        // Apply movement force
        rigidbody.AddRelativeForce(Vector3.up * Time.deltaTime * movementSpeed);
        
        // Handle audio and particles
        HandleAudioAndParticles(audioSource);
    }

    private bool HandleFuelConsumption(AudioSource audioSource)
    {
        if (fuelManager == null || !fuelManager.IsUsingFuel)
        {
            return true; // No fuel system or fuel not required
        }
        
        if (fuelManager.FuelAmount <= 0)
        {
            // Stop audio and particles when out of fuel
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            
            if (rocketBoostParticles != null && rocketBoostParticles.isPlaying)
            {
                rocketBoostParticles.Stop();
            }
            
            return false; // No fuel available
        }
        
        // Consume fuel
        fuelManager.FuelConsumption(1);
        return true;
    }

    private void HandleAudioAndParticles(AudioSource audioSource)
    {
        // Handle audio
        if (audioSource != null && mainEngine != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        
        // Handle particles
        if (rocketBoostParticles != null && !rocketBoostParticles.isPlaying)
        {
            rocketBoostParticles.Play();
        }
    }
}
