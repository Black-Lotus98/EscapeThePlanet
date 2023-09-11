using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUp : Command
{
    private float movementSpeed;
    private AudioClip mainEngine;
    private ParticleSystem rocketBoostParticles;

    private Player player;

    public MoveUp(float movementSpeed, AudioClip mainEngine, ParticleSystem rocketBoostParticles, Player player)
    {
        this.movementSpeed = movementSpeed;
        this.mainEngine = mainEngine;
        this.rocketBoostParticles = rocketBoostParticles;
        this.player = player;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        if (player.GetIsUsingFuel())
        {
            player.FuelUsage(-1);
            player.NotifyObservers(Player.PlayerState.FuelChanged);
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
