using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRight : Command
{
    private readonly float rotationSpeed;
    private readonly ParticleSystem rightThrustParticles;

    public RotateRight(float rotationSpeed, ParticleSystem rightThrustParticles)
    {
        this.rotationSpeed = rotationSpeed;
        this.rightThrustParticles = rightThrustParticles;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        ApplyRotation(rigidbody, -1);
        PlayParticles();
    }

    private void ApplyRotation(Rigidbody rigidbody, float rotation)
    {
        rigidbody.transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed * rotation);
    }

    private void PlayParticles()
    {
        if (rightThrustParticles != null && !rightThrustParticles.isPlaying)
        {
            rightThrustParticles.Play();
        }
    }
}
