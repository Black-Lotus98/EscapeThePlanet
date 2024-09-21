using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopThrust : Command
{
    private readonly AudioClip mainEngine;
    private readonly ParticleSystem rocketBoostParticles;

    public StopThrust(AudioClip mainEngine, ParticleSystem rocketBoostParticles)
    {
        this.mainEngine = mainEngine;
        this.rocketBoostParticles = rocketBoostParticles;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        // Stop particles
        if (rocketBoostParticles != null && rocketBoostParticles.isPlaying)
        {
            rocketBoostParticles.Stop();
        }
        
        // Stop audio
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
