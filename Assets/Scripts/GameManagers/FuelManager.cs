using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FuelManager : UIManager, IUIObservable<FuelManager>, ICheckpointable
{
    [Header("Fuel Settings")]
    [SerializeField] private bool isUsingFuel = false;
    [SerializeField] private float fuelAmount;
    [SerializeField] private float maxFlightTime = 0;
    [SerializeField] private AudioClip fuelCollectableSound;

    // Getters and Setters
    public bool IsUsingFuel
    {
        get { return isUsingFuel; }
    }

    private bool tankScaled;

    public float FuelAmount
    {
        get { EnsureTankScaled(); return fuelAmount; }
        set
        {
            EnsureTankScaled();
            fuelAmount = Mathf.Clamp(value, 0, MaxFlightTime);
            NotifyObservers(UIState.FuelChanged);
        }
    }

    public float MaxFlightTime
    {
        get { return DifficultyRuntime.FuelCapacity(maxFlightTime); }
    }

    private void EnsureTankScaled()
    {
        if (tankScaled) return;
        tankScaled = true;
        fuelAmount = DifficultyRuntime.FuelCapacity(fuelAmount);
    }

    private readonly List<IUIObserver<FuelManager>> observers = new List<IUIObserver<FuelManager>>();
    
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
            if (observer != null)
            {
                observer.OnStateChange(this, state);
            }
            else
            {
                Debug.LogWarning("Null observer found in FuelManager observers list!");
            }
        }
    }

    public void FuelConsumption(float amount)
    {
        if (!IsUsingFuel)
        {
            return;
        }

        if (FuelAmount <= 0)
        {
            FuelAmount = 0;
            return;
        }

        FuelAmount -= amount * DifficultyRuntime.FuelConsumption(1f) * Time.deltaTime;
    }

    public void FuelBarrel(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (FuelAmount >= MaxFlightTime)
        {
            FuelAmount = MaxFlightTime;
            return;
        }

        if (CollectableAS != null && fuelCollectableSound != null)
        {
            CollectableAS.PlayOneShot(fuelCollectableSound);
        }
        
        FuelAmount += amount;
    }

    public void ExecutePowerUp(ICollectibleBehavior<FuelManager> collectableBehaviour)
    {
        if (collectableBehaviour == null)
        {
            Debug.LogWarning("CollectableBehaviour is null in ExecutePowerUp.");
            return;
        }

        collectableBehaviour.ExecutePowerUp(this);
    }

    public void RefillFuel(float refillSpeed)
    {
        if (refillSpeed <= 0)
        {
            return;
        }

        // Using the * refillSpeed to make the refill depend on how fast the player is refilling
        FuelAmount += Time.deltaTime * refillSpeed;
        isUsingFuel = true;
    }

    // Memento Pattern: snapshot/restore the fuel level at a checkpoint.
    private class FuelMemento
    {
        public float amount;
        public bool usingFuel;
    }

    public object CaptureState()
    {
        EnsureTankScaled();
        return new FuelMemento { amount = fuelAmount, usingFuel = isUsingFuel };
    }

    public void RestoreState(object memento)
    {
        if (memento is FuelMemento fuelMemento)
        {
            tankScaled = true;
            fuelAmount = Mathf.Clamp(fuelMemento.amount, 0, MaxFlightTime);
            isUsingFuel = fuelMemento.usingFuel;
            NotifyObservers(UIState.FuelChanged);
        }
    }
}
