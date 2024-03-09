using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour
{
    [SerializeField] Vector3 Rotaion;
    [SerializeField] float RotationSpeed = 150f;
    void Update()
    {
        transform.Rotate(Rotaion * Time.deltaTime * RotationSpeed, Space.World);
    }
}