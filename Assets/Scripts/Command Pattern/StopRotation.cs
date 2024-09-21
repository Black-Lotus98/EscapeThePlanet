using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRotation : Command
{
    private readonly ParticleSystem leftThrustParticles;
    private readonly ParticleSystem rightThrustParticles;

    public StopRotation(ParticleSystem leftThrustParticles, ParticleSystem rightThrustParticles)
    {
        this.leftThrustParticles = leftThrustParticles;
        this.rightThrustParticles = rightThrustParticles;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        // Stop left thrust particles
        if (leftThrustParticles != null && leftThrustParticles.isPlaying)
        {
            leftThrustParticles.Stop();
        }
        
        // Stop right thrust particles
        if (rightThrustParticles != null && rightThrustParticles.isPlaying)
        {
            rightThrustParticles.Stop();
        }
    }
}
