using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLeft : Command
{
    private readonly float rotationSpeed;
    private readonly ParticleSystem leftThrustParticles;

    public RotateLeft(float rotationSpeed, ParticleSystem leftThrustParticles)
    {
        this.rotationSpeed = rotationSpeed;
        this.leftThrustParticles = leftThrustParticles;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        ApplyRotation(rigidbody, 1);
        PlayParticles();
    }

    private void ApplyRotation(Rigidbody rigidbody, float rotation)
    {
        rigidbody.transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed * rotation);
    }

    private void PlayParticles()
    {
        if (leftThrustParticles != null && !leftThrustParticles.isPlaying)
        {
            leftThrustParticles.Play();
        }
    }
}
