using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class ShieldManager : UIManager, IUIObservable<ShieldManager>
{
    [Header("Shield Settings")]
    [SerializeField] bool shieldAllowed = false;
    [SerializeField] GameObject shield;
    [SerializeField] AudioClip shieldActivationSound;
    [SerializeField] AudioClip shieldCollectableSound;
    [SerializeField] float shieldMaxTime;
    [SerializeField] float currentShieldTime;
    bool shieldIsActive = false;

    public float ShieldMaxTime
    {
        get { return this.shieldMaxTime; }
    }

    public float CurrentShieldTime
    {
        get { return this.currentShieldTime; }
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
        get
        {
            return shieldAllowed;
        }
        // set
        // {
        //     if (shieldAllowed != value)
        //     {
        //         shieldAllowed = value;
        //         NotifyObservers(UIState.ShieldChanged);
        //     }
        // }
    }
    public bool ShieldIsActive
    {
        get { return this.shieldIsActive; }
    }

    public void ToggleShield()
    {
        if (shieldAllowed)
        {
            shieldIsActive = !shieldIsActive;
            if (currentShieldTime > 0.0f)
            {
                if (shieldIsActive)
                {
                    shield.SetActive(true);
                    AS.PlayOneShot(shieldActivationSound);
                }
                else
                {
                    AS.Stop();
                    shield.SetActive(false);
                }
            }
            else
            {
                shield.SetActive(false);
            }
        }
        else
        {
            shield.SetActive(false);
        }
    }

    private void Update()
    {
        {
            if (shield.activeInHierarchy)
            {
                if (currentShieldTime > 0.0f)
                {
                    currentShieldTime -= Time.deltaTime;
                    NotifyObservers(UIState.ShieldChanged);
                }
                else
                {
                    AS.Stop();
                    currentShieldTime = 0.0f;
                    shield.SetActive(false);
                    NotifyObservers(UIState.ShieldChanged);
                }
            }
        }
    }


    private List<IUIObserver<ShieldManager>> observers = new List<IUIObserver<ShieldManager>>();

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
            observer.OnStateChange(this, state);
        }
    }

    public void IncreaseShieldTime(float amount)
    {
        CollectableAS.PlayOneShot(shieldCollectableSound);
        currentShieldTime += amount;
        NotifyObservers(UIState.ShieldChanged);
        // ShieldAllowed = true;
    }



}
