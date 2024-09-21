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
        if (leftThrustParticles != null && !leftThrustParticles.isPlaying)
        {
            leftThrustParticles.Play();
        }
    }
}
