using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShieldManager : UIManager, IUIObservable<ShieldManager>, ICheckpointable
{
    [Header("Shield Settings")]
    [SerializeField] private bool shieldAllowed = false;
    [SerializeField] private GameObject shield;
    [SerializeField] private AudioClip shieldActivationSound;
    [SerializeField] private AudioClip shieldCollectableSound;
    [SerializeField] private float shieldMaxTime;
    [SerializeField] private float currentShieldTime;
    
    private bool shieldIsActive = false;

    public float ShieldMaxTime
    {
        get { return shieldMaxTime; }
    }

    public float CurrentShieldTime
    {
        get { return currentShieldTime; }
        set
        {
            if (currentShieldTime != value)
            {
                currentShieldTime = value;
                NotifyObservers(UIState.ShieldChanged);
            }
        }
    }

    public bool ShieldAllowed
    {
        get { return shieldAllowed; }
    }
    
    public bool ShieldIsActive
    {
        get { return shieldIsActive; }
    }

    public void ToggleShield()
    {
        if (!shieldAllowed || shield == null)
        {
            if (shield != null)
            {
                shield.SetActive(false);
            }
            return;
        }

        shieldIsActive = !shieldIsActive;
        
        if (currentShieldTime > 0.0f)
        {
            if (shieldIsActive)
            {
                shield.SetActive(true);
                if (AS != null && shieldActivationSound != null)
                {
                    AS.PlayOneShot(shieldActivationSound);
                }
            }
            else
            {
                if (AS != null && AS.isPlaying)
                {
                    AS.Stop();
                }
                shield.SetActive(false);
            }
        }
        else
        {
            shield.SetActive(false);
            shieldIsActive = false;
        }
    }

    private void Update()
    {
        // Only process if shield exists and is active
        if (shield == null || !shield.activeInHierarchy)
        {
            return;
        }

        if (currentShieldTime > 0.0f)
        {
            currentShieldTime -= Time.deltaTime;
            NotifyObservers(UIState.ShieldChanged);
        }
        else
        {
            // Shield time depleted
            if (AS != null && AS.isPlaying)
            {
                AS.Stop();
            }
            currentShieldTime = 0.0f;
            shield.SetActive(false);
            shieldIsActive = false;
            NotifyObservers(UIState.ShieldChanged);
        }
    }

    private readonly List<IUIObserver<ShieldManager>> observers = new List<IUIObserver<ShieldManager>>();

    public void AddObserver(IUIObserver<ShieldManager> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IUIObserver<ShieldManager> observer)
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
                Debug.LogWarning("Null observer found in ShieldManager observers list!");
            }
        }
    }

    public void IncreaseShieldTime(float amount)
    {
        if (CollectableAS != null && shieldCollectableSound != null)
        {
            CollectableAS.PlayOneShot(shieldCollectableSound);
        }
        
        currentShieldTime += amount;
        NotifyObservers(UIState.ShieldChanged);
    }

    // Memento Pattern: snapshot/restore the shield charge and active state at a checkpoint.
    private class ShieldMemento
    {
        public float time;
        public bool allowed;
        public bool active;
        public bool shieldObjectActive;
    }

    public object CaptureState()
    {
        return new ShieldMemento
        {
            time = currentShieldTime,
            allowed = shieldAllowed,
            active = shieldIsActive,
            shieldObjectActive = shield != null && shield.activeSelf,
        };
    }

    public void RestoreState(object memento)
    {
        if (memento is ShieldMemento shieldMemento)
        {
            currentShieldTime = shieldMemento.time;
            shieldAllowed = shieldMemento.allowed;
            shieldIsActive = shieldMemento.active;
            if (shield != null)
            {
                shield.SetActive(shieldMemento.shieldObjectActive);
            }
            NotifyObservers(UIState.ShieldChanged);
        }
    }
}
