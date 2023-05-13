using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRight : Command
{
    private float rotationSpeed;
    private ParticleSystem rightThrustParticles;

    public RotateRight(float rotationSpeed, ParticleSystem rightThrustParticles)
    {
        this.rotationSpeed = rotationSpeed;
        this.rightThrustParticles = rightThrustParticles;
    }

    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        ApplyRotation(rigidbody, -1);
        if (!rightThrustParticles.isPlaying)
        {
            rightThrustParticles.Play();
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
