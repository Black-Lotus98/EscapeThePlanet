using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUp : Command
{
    private float movementSpeed;
    private AudioClip mainEngine;
    private ParticleSystem rocketBoostParticles;

    public MoveUp(float movementSpeed, AudioClip mainEngine, ParticleSystem rocketBoostParticles)
    {
        this.movementSpeed = movementSpeed;
        this.mainEngine = mainEngine;
        this.rocketBoostParticles = rocketBoostParticles;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
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
