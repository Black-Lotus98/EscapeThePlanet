using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopRotation : Command
{
    private ParticleSystem leftThrustParticles;
    private ParticleSystem rightThrustParticles;

    public StopRotation(ParticleSystem leftThrustParticles, ParticleSystem rightThrustParticles)
    {
        this.leftThrustParticles = leftThrustParticles;
        this.rightThrustParticles = rightThrustParticles;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        leftThrustParticles.Stop();
        rightThrustParticles.Stop();
    }
}
