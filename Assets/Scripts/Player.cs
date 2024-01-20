using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;


public class Player : MonoBehaviour, IObservable
{
    [SerializeField] AudioSource AS;
    [SerializeField] AudioSource CollectableAS;

    // The fuel bar is relying on the observer pattern to be updated on the UI 
    // as a result we don't need to reference the UI element in the scene editor
    [Header("Fuel Settings")]
    [SerializeField] bool useFuel = false;
    // [SerializeField] Slider FuelSlider;
    [SerializeField] float FuelCounter;
    [SerializeField] float MaxFlightTime = 0;
    [SerializeField] AudioClip FuelBarrelCollectableSound;



    [Header("Shield Settings")]
    [SerializeField] bool shieldCanBeUsed = false;
    public bool ShieldCanBeUsed
    {
        get
        {
            return shieldCanBeUsed;
        }
        set
        {
            if (shieldCanBeUsed != value)
            {
                shieldCanBeUsed = value;
                NotifyObservers(UIState.ShieldChanged);
                // NotifyObservers(PlayerState.ShieldChanged);
                // NotifyObservers(PlayerState.FuelChanged);
            }
        }
    }

    [SerializeField] GameObject Shield;
    [SerializeField] AudioClip ShieldActivationSound;
    [SerializeField] float ShieldMaxTime;
    [SerializeField] float CurrentShieldTime;
    // [SerializeField] Slider ShieldSlider;
    // [SerializeField] TextMeshProUGUI ShieldText;
    bool shieldIsActive = false;

    [Header("Other Settings")]
    [SerializeField] bool playerHasKey = false;

    public int StarsCollected = 0;


    [Header("Game Managers")]
    [SerializeField] ShieldManager shieldManager;
    SaveDataManager saveDataManager;

    public ShieldManager GetShieldManager()
    {
        return shieldManager;
    }

    // The enums are used to prevent the observer from 
    // being notified whenever the state of the shield or fuel changes
    // so the enum will be used to check the state of the shield and fuel independently
    // To access the state of the shield and fuel use in the observers I must use Player.PlayerState.ENUMVALUE
    // For example: Player.PlayerState.ShieldChanged or Player.PlayerState.FuelChanged
    // public enum PlayerState
    // {
    //     FuelChanged,
    //     ShieldChanged,
    //     KeyState,
    //     StarsState,
    // }

    private List<IPlayerObserver> observers = new List<IPlayerObserver>();

    public void AddObserver(IPlayerObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IPlayerObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers(UIState state)
    {
        foreach (IPlayerObserver observer in observers)
        {
            observer.OnPlayerStateChange(this, state);
        }
    }
    // public void NotifyObservers(PlayerState state)
    // {
    //     foreach (IPlayerObserver observer in observers)
    //     {
    //         observer.OnPlayerStateChange(this, state);
    //     }
    // }

    public void ExecutePowerUp(ICollectibleBehavior<UIManager> collectableBehaviour)
    {
        // collectableBehaviour.ExecutePowerUp(this);
        NotifyObservers(UIState.ShieldChanged);
        NotifyObservers(UIState.FuelChanged);
        // NotifyObservers(PlayerState.ShieldChanged);
        // NotifyObservers(PlayerState.FuelChanged);
    }




    void Start()
    {
        // This will look for the save data manager in the scene, but since we are using the singleton pattern,
        // we don't need it because we can access it globally
        // saveDataManager = GameObject.Find("SaveDataManager").GetComponent<SaveDataManager>();

        saveDataManager = SaveDataManager.Instance;
        NotifyObservers(UIState.ShieldChanged);
        NotifyObservers(UIState.FuelChanged);

        // Part of old way to update shield and fuel ui
        // NotifyObservers(PlayerState.ShieldChanged);
        // NotifyObservers(PlayerState.FuelChanged);
        // UpdateShieldUi();
    }

    // void Awake()
    // {
    //     Stars = FindObjectsOfType<Star>();
    // }

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
                    shieldIsActive = true;
                    AS.PlayOneShot(ShieldActivationSound);

                }
                else
                {
                    shieldIsActive = false;
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
                NotifyObservers(UIState.ShieldChanged);
                // NotifyObservers(PlayerState.ShieldChanged);
            }
            else
            {
                AS.Stop();
                CurrentShieldTime = 0.0f;
                ShieldCanBeUsed = false;
                NotifyObservers(UIState.ShieldChanged);
                // NotifyObservers(PlayerState.ShieldChanged);
            }

        }
    }

    public void IncreaseShieldTime(float amount)
    {
        CollectableAS.PlayOneShot(FuelBarrelCollectableSound);
        CurrentShieldTime += amount;
        NotifyObservers(UIState.ShieldChanged);
        // NotifyObservers(PlayerState.ShieldChanged);
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




    public void FuelUsage(float amount)
    {
        if (useFuel)
        {
            if (FuelCounter > MaxFlightTime)
            {
                FuelCounter = MaxFlightTime;
            }
            else
            {
                FuelCounter += amount * Time.deltaTime;
            }
            NotifyObservers(UIState.FuelChanged);
            // NotifyObservers(PlayerState.FuelChanged);
        }
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
        NotifyObservers(UIState.FuelChanged);
        // NotifyObservers(PlayerState.FuelChanged);
    }

    public bool GetIsUsingFuel()
    {
        return this.useFuel;
    }

    public bool GetIsUsingShield()
    {
        return this.shieldIsActive;
    }

    // This method is to stop the IShieldObserver form Executing the commands
    public float GetFuelCounter()
    {
        return this.FuelCounter;
    }


    public float GetMaxFlightTime()
    {
        return this.MaxFlightTime;
    }


    // public int GetNumberOfStars()
    // {
    //     return Stars.Length;
    // }

    // public void UpdateStarsGUI()
    // {
    //     Stars[collectedStarsCounter].GetComponent<Image>().sprite = CollectedStar;
    // }

    // public bool GetStarStatus(int index)
    // {
    //     if (Stars[index].gameObject.GetComponent<Image>().sprite != UncollectedStar)
    //     {
    //         starStatus = false;
    //     }
    //     return starStatus;
    // }

    public int GetStarCount()
    {
        GameData gameData = SaveManager.Load();
        var currentLevelData = GetLevelData(gameData);

        return currentLevelData.collectedStars;
    }

    public void SetCollectedStarsCounter(int amount)
    {
        saveDataManager.SaveCollectedStar();

        CollectableAS.PlayOneShot(FuelBarrelCollectableSound);
        // NotifyObservers(PlayerState.StarsState);
        NotifyObservers(UIState.StarsState);

    }

    public bool GetKeyStatus()
    {
        return playerHasKey;
    }

    public void SetKeyStatus(bool status)
    {
        // UpdateKeyGUI();
        CollectableAS.PlayOneShot(FuelBarrelCollectableSound);
        NotifyObservers(UIState.KeyState);
        // NotifyObservers(PlayerState.KeyState);
        playerHasKey = status;
    }

    LevelData GetLevelData(GameData gameData)
    {
        var sceneData = SceneManager.GetActiveScene();

        // using .buildIndex to get the scene index and .name to get the scene name
        var currentLevelData = gameData.levelData.Where(x => x.currentLevelIndex == sceneData.buildIndex).FirstOrDefault();
        if (currentLevelData == null)
        {
            currentLevelData = new LevelData(sceneData.name, sceneData.buildIndex);
        }
        return currentLevelData;
    }

    // Unused methods
    // this method is no longer needed because of the observer pattern
    // public void UpdateFuelUi()
    // {
    //     FuelSlider.maxValue = MaxFlightTime;
    //     FuelSlider.value = FuelCounter;
    //     NotifyObservers();
    // }

    // public void SetFuelCounter(float amount)
    // {
    //     this.FuelCounter = amount;
    // }

    // void UpdateShieldUi()
    // {
    //     ShieldSlider.maxValue = ShieldMaxTime;
    //     ShieldSlider.value = CurrentShieldTime;

    //     ShieldText.text = (Mathf.Round(CurrentShieldTime * 100.00f) * 0.01f).ToString() + "/" + ShieldMaxTime.ToString();
    // }

}
