using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    [SerializeField] Vector3 Rotation;
    [SerializeField] float RotationSpeed = 150f;
    void Update()
    {
        transform.Rotate(Rotation * Time.deltaTime * RotationSpeed, Space.World);
    }
}