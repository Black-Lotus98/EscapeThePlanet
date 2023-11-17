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
        if (isUsingFuel)
        {
            fuelAmount += amount * Time.deltaTime;
            NotifyObservers(UIState.FuelChanged);
            // NotifyObservers(PlayerState.FuelChanged);
        }
    }

    public float MaxFlightTime
    {
        get { return this.maxFlightTime; }
    }

    public void FuelBarrel(int amount)
    {
        if (fuelAmount > maxFlightTime)
        {
            fuelAmount = maxFlightTime;
        }
        else
        {
            CollectableAS.PlayOneShot(fuelCollectableSound);
            fuelAmount += amount;

        }
        NotifyObservers(UIState.FuelChanged);
    }

    public void ExecutePowerUp(ICollectibleBehavior<FuelManager> collectableBehaviour)
    {
        collectableBehaviour.ExecutePowerUp(this);
        NotifyObservers(UIState.FuelChanged);
    }

}
