using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MovementController : MonoBehaviour
{
    // Component References
    public Rigidbody Rigidbdy { get; private set; }
    public AudioSource AS { get; private set; }

    private void Start()
    {
        // Cache component references for better performance
        Rigidbdy = GetComponent<Rigidbody>();
        AS = GetComponent<AudioSource>();
        
        // Validate required components
        if (Rigidbdy == null)
        {
            Debug.LogError("Rigidbody component not found on MovementController GameObject.");
            enabled = false;
            return;
        }
        
        if (AS == null)
        {
            Debug.LogWarning("AudioSource component not found on MovementController GameObject.");
        }
    }
}