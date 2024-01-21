using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "FuelPad Data")]

public class FuelPadData : ScriptableObject
{
    [SerializeField] float refillAmount;
    [SerializeField] float refillSpeed;

    public float RefillAmount { get => refillAmount; set => refillAmount = value; }
    public float RefillSpeed { get => refillSpeed; set => refillSpeed = value; }

    public FuelPadData()
    {
        refillAmount = 10f;
        refillSpeed = 1.0f;
    }
}
