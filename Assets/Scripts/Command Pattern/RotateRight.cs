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
        // Cache transform reference for better performance
        Transform transform = rigidbody.transform;
        
        // Lock Y rotation to prevent side-to-side movement, allow X and Z rotation
        Vector3 currentRotation = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation.x, 0, currentRotation.z);
        
        // Apply rotation using physics
        rigidbody.freezeRotation = true;
        transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed * rotation);
        rigidbody.freezeRotation = false;
    }

    private void PlayParticles()
    {
        if (rightThrustParticles != null && !rightThrustParticles.isPlaying)
        {
            rightThrustParticles.Play();
        }
    }
}
