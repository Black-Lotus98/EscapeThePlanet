using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLeft : Command
{
    private float rotationSpeed;
    private ParticleSystem leftThrustParticles;

    public RotateLeft(float rotationSpeed, ParticleSystem leftThrustParticles)
    {
        this.rotationSpeed = rotationSpeed;
        this.leftThrustParticles = leftThrustParticles;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        ApplyRotation(rigidbody, 1);
        if (!leftThrustParticles.isPlaying)
        {
            leftThrustParticles.Play();
        }
    }

    private void ApplyRotation(Rigidbody rigidbody, float rotation)
    {
        Transform transform = rigidbody.transform;
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, 0, transform.eulerAngles.z);
        rigidbody.freezeRotation = true;
        transform.Rotate(Vector3.forward * Time.deltaTime * rotationSpeed * rotation);
        rigidbody.freezeRotation = false;
    }
}
