using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscllator : MonoBehaviour
{
    Vector3 StartingPosition;
    [SerializeField] Vector3 movementsVector;
    [SerializeField] [Range(-1,1)] float movementsFactor;
    [SerializeField] float period = 5f;
    [SerializeField] bool fullcycles= false;

    // Start is called before the first frame update
    void Start()
    {
        StartingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(period <= Mathf.Epsilon)
        {
            return;
        }
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles*tau);

        if(fullcycles)
        {
           
           movementsFactor = rawSinWave; 
        }
        else
        {
            movementsFactor = (rawSinWave + 1f)/2f;
        }

        Vector3 offset = movementsVector * movementsFactor;
        transform.position = StartingPosition + offset;

    }
}
