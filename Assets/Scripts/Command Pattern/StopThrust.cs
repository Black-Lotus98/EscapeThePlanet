using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopThrust : Command
{
    private AudioClip mainEngine;
    private ParticleSystem rocketBoostParticles;

    public StopThrust(AudioClip mainEngine, ParticleSystem rocketBoostParticles)
    {
        this.mainEngine = mainEngine;
        this.rocketBoostParticles = rocketBoostParticles;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        rocketBoostParticles.Stop();
        audioSource.Stop();
    }
}
