using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPad : MonoBehaviour
{
    public float refillAmount;
    float refillSpeed;

    [SerializeField] FuelPadState currentState;
    public FuelPadData fuelPadData;
    public FuelManager fuelManager;
    [SerializeField] AudioClip FuelStationSound;

    AudioSource AS;


    // The coroutine is used because the the fuel refill is a happing over time. 
    // In Unity, a coroutine is a method that can pause execution and return control 
    // to Unity but then continue where it left off on the following frame.
    private Coroutine fuelRefillCoroutine;


    private void Awake()
    {
        ChangeState(new IdleState());
        refillAmount = fuelPadData.RefillAmount;
        refillSpeed = fuelPadData.RefillSpeed;
        AS = GetComponent<AudioSource>();
        // fuelManager = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<FuelManager>();
    }

    public void ActivateFuelRefill()
    {
        if (refillAmount >= 0 && fuelManager.FuelAmount < fuelManager.MaxFlightTime)
        {
            fuelManager.RefillFuel(refillAmount);
            Debug.Log("RefillSpeed: " + refillAmount);
            refillAmount -= Time.deltaTime * refillAmount;
            Debug.Log("RefillAmount: " + refillAmount);

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeState(new ActiveFuelPadState());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ChangeState(new IdleState());
        }
    }


    public void ChangeState(FuelPadState newState)
    {
        // The ? is used instead of using the if statement ( if (currentState != null) { currentState.DeactivateState(this); } )
        currentState?.DeactivateState(this);
        currentState = newState;
        currentState.ActivateState(this);
    }


    public void StartFuelRefill()
    {
        if (fuelRefillCoroutine != null)
        {
            StopCoroutine(fuelRefillCoroutine);
        }
        fuelRefillCoroutine = StartCoroutine(RefillFuel());
    }

    public void StopFuelRefill()
    {
        if (fuelRefillCoroutine != null)
        {
            StopCoroutine(fuelRefillCoroutine);
            AS.loop = false;
            AS.Stop();
        }

    }

    // The IEnumerator is used to refill the fuel over time because it is a coroutine and not a function
    private IEnumerator RefillFuel()
    {
        // Play audio when refilling
        if (refillAmount > 0)
        {
            AS.clip = FuelStationSound;
            AS.loop = true;
            AS.Play();
        }

        while (refillAmount > 0 && fuelManager.FuelAmount < fuelManager.MaxFlightTime)
        {
            fuelManager.RefillFuel(refillSpeed);
            refillAmount -= Time.deltaTime * refillSpeed;
            yield return null;
        }

        // Stop playing audio when refilling stops and disable looping
        AS.loop = false;
        AS.Stop();
    }
}
