using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FuelManager : UIManager, IUIObservable<FuelManager>
{
    [Header("Fuel Settings")]
    [SerializeField] bool isUsingFuel = false;
    [SerializeField] float fuelAmount;
    [SerializeField] float maxFlightTime = 0;
    [SerializeField] AudioClip fuelCollectableSound;



    // Getters and Setters
    public bool IsUsingFuel
    {
        get { return this.isUsingFuel; }
    }

    public float FuelAmount
    {
        get { return this.fuelAmount; }
        set { this.fuelAmount = value; }
    }

    private List<IUIObserver<FuelManager>> observers = new List<IUIObserver<FuelManager>>();
    public void AddObserver(IUIObserver<FuelManager> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IUIObserver<FuelManager> observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    public new void NotifyObservers(UIState state)
    {
        foreach (var observer in observers)
        {
            observer.OnStateChange(this, state);
        }
    }
    public void FuelConsumption(float amount)
    {
        if (IsUsingFuel)
        {
            if (FuelAmount <= 0)
            {
                FuelAmount = 0;
            }
            else
            {
                FuelAmount -= amount * Time.deltaTime;
            }
            NotifyObservers(UIState.FuelChanged);
        }
    }

    public float MaxFlightTime
    {
        get { return this.maxFlightTime; }
    }

    public void FuelBarrel(int amount)
    {
        if (FuelAmount > maxFlightTime)
        {
            FuelAmount = maxFlightTime;
        }
        else
        {
            CollectableAS.PlayOneShot(fuelCollectableSound);
            FuelAmount += amount;

        }
        NotifyObservers(UIState.FuelChanged);
    }

    public void ExecutePowerUp(ICollectibleBehavior<FuelManager> collectableBehaviour)
    {
        collectableBehaviour.ExecutePowerUp(this);
        NotifyObservers(UIState.FuelChanged);
    }

    public void RefillFuel(float refillSpeed)
    {
        // Using the * refillSpeed to make the refill depend on how fast the player is refilling
        FuelAmount += Time.deltaTime * refillSpeed;
        isUsingFuel = true;
        NotifyObservers(UIState.FuelChanged);
    }

}
