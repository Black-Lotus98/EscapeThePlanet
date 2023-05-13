using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using TMPro;


public class Player : MonoBehaviour
{
    [SerializeField] AudioSource AS;
    [SerializeField] AudioSource CollectableAS;


    [Header("Fuel Settings")]
    [SerializeField] bool UseFuel = false;
    [SerializeField] Slider FuelSlider;
    [SerializeField] float FuelCounter;
    [SerializeField] float MaxFlightTime = 0;
    [SerializeField] AudioClip FuelBarrelCollectableSound;



    [Header("Shield Settings")]
    [SerializeField] bool ShieldCanBeUsed = false;
    [SerializeField] GameObject Shield;
    [SerializeField] AudioClip ShieldActivationSound;
    [SerializeField] float ShieldMaxTime;
    [SerializeField] float CurrentShieldTime;
    [SerializeField] Slider ShieldSlider;
    [SerializeField] TextMeshProUGUI ShieldText;
    bool shieldIsActive;





    [Header("Other Settings")]
    [SerializeField] Image[] Stars;
    [SerializeField] Sprite CollectedStar;
    [SerializeField] Sprite UncollectedStar;
    int collectedStarsCounter = 0;
    bool starStatus = true;
    [SerializeField] Image Key;
    [SerializeField] Sprite CollectedKey;
    [SerializeField] bool playerHasKey = false;


    private List<IPlayerObserver> observers = new List<IPlayerObserver>();

    public void AddObserver(IPlayerObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IPlayerObserver observer)
    {
        observers.Remove(observer);
    }

    private void NotifyObservers()
    {
        foreach (IPlayerObserver observer in observers)
        {
            observer.OnPlayerStateChange(this);
        }
    }

    public void ExecutePowerUp(IPowerUpStrategy powerUpStrategy)
    {
        powerUpStrategy.ExecutePowerUp(this);
        NotifyObservers();
    }




    void Start()
    {
        UpdateShieldUi();
    }

    void UpdateShieldUi()
    {
        ShieldSlider.maxValue = ShieldMaxTime;
        ShieldSlider.value = CurrentShieldTime;

        ShieldText.text = (Mathf.Round(CurrentShieldTime * 100.00f) * 0.01f).ToString() + "/" + ShieldMaxTime.ToString();
    }

    void Update()
    {

        if (gameObject.GetComponent<InputHandler>().enabled)
        {
            ActivateShield();
            shieldUsage();
        }
        else
        {
            AS.Stop();
            return;
        }
    }

    void ActivateShield()
    {
        if (ShieldCanBeUsed)
        {
            if (Input.GetKeyDown(KeyCode.E) || CrossPlatformInputManager.GetButtonDown("Shield"))
            {

                if (!Shield.activeInHierarchy)
                {
                    Shield.SetActive(true);
                    AS.PlayOneShot(ShieldActivationSound);

                }
                else
                {
                    AS.Stop();
                    Shield.SetActive(false);
                }
            }
        }
        else
        {
            Shield.SetActive(false);
        }
    }

    void shieldUsage()
    {
        if (Shield.activeInHierarchy)
        {
            if (CurrentShieldTime > 0.0f)
            {
                CurrentShieldTime -= Time.deltaTime;
                UpdateShieldUi();

            }
            else
            {
                AS.Stop();
                CurrentShieldTime = 0.0f;
                UpdateShieldUi();
                ShieldCanBeUsed = false;
            }
        }
    }

    public void IncreaseShieldTime(float amount)
    {
        CollectableAS.PlayOneShot(FuelBarrelCollectableSound);
        CurrentShieldTime += amount;
        UpdateShieldUi();
        ShieldCanBeUsed = true;
    }

    public float GetCurrentShieldTime()
    {
        return this.CurrentShieldTime;
    }
    public float GetShieldMaxTime()
    {
        return this.ShieldMaxTime;
    }


    public void UpdateFuelUi()
    {
        FuelSlider.maxValue = MaxFlightTime;
        FuelSlider.value = FuelCounter;
    }


    public void FuelUsage(float amount)
    {
        if (FuelCounter > MaxFlightTime)
        {
            FuelCounter = MaxFlightTime;
        }
        else
        {
            FuelCounter += amount * Time.deltaTime;
        }
        UpdateFuelUi();
    }


    public void FuelBarrel(int amount)
    {
        if (FuelCounter > MaxFlightTime)
        {
            FuelCounter = MaxFlightTime;
        }
        else
        {
            CollectableAS.PlayOneShot(FuelBarrelCollectableSound);
            FuelCounter += amount;

        }
        NotifyObservers();
    }

    public bool GetUsingFuelStatus()
    {
        return this.UseFuel;
    }

    public float GetFuelCounter()
    {
        return this.FuelCounter;
    }

    public void SetFuelCounter(float amount)
    {
        this.FuelCounter = amount;
    }
    public float GetMaxFlightTime()
    {
        return this.MaxFlightTime;
    }


    public int GetNumberOfStars()
    {
        return Stars.Length;
    }

    public void UpdateStarsGUI()
    {
        Stars[collectedStarsCounter].GetComponent<Image>().sprite = CollectedStar;
    }

    public bool GetStarStatus(int index)
    {
        if (Stars[index].gameObject.GetComponent<Image>().sprite != UncollectedStar)
        {
            starStatus = false;
        }
        return starStatus;
    }

    public int GetCollectedStarsCounter()
    {
        return collectedStarsCounter;
    }

    public void SetCollectedStarsCounter(int amount)
    {
        this.collectedStarsCounter += amount;
    }

    public void UpdateKeyGUI()
    {
        Key.GetComponent<Image>().sprite = CollectedKey;
    }

    public bool GetKeyStatus()
    {
        return playerHasKey;
    }

    public void SetKeyStatus(bool status)
    {
        UpdateKeyGUI();
        playerHasKey = status;
    }
}
