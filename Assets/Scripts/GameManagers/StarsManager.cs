using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarsManager : UIManager, IUIObservable<StarsManager>
{
    [Header("Star Settings")]
    [SerializeField] private AudioClip starCollectableSound;
    
    private int collectedStarsCounter = 0;
    private const int MAX_STARS = 3;

    private void Start()
    {
        if (saveDataManager == null)
        {
            Debug.LogError("SaveDataManager is null in StarsManager!");
            return;
        }

        try
        {
            LevelData currentLevelData = GetLevelData(saveDataManager.Load());
            if (currentLevelData != null)
            {
                saveDataManager.TempCollectedStars = currentLevelData.collectedStars;
                currentLevelData.collectedStars = 0;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error initializing StarsManager: {e.Message}");
        }
    }

    public int CollectedStarsCounter
    {
        get { return collectedStarsCounter; }
        set
        {
            if (collectedStarsCounter >= MAX_STARS)
            {
                Debug.LogWarning("Maximum stars already collected!");
                return;
            }

            if (value <= 0)
            {
                Debug.LogWarning("Invalid star value provided!");
                return;
            }

            try
            {
                LevelData currentLevelData = GetLevelData(saveDataManager.Load());
                if (currentLevelData != null)
                {
                    collectedStarsCounter += value;
                    saveDataManager.SaveCollectedStar();
                    
                    if (AS != null && starCollectableSound != null)
                    {
                        AS.PlayOneShot(starCollectableSound);
                    }
                    
                    NotifyObservers(UIState.StarsState);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error collecting star: {e.Message}");
            }
        }
    }

    private readonly List<IUIObserver<StarsManager>> observers = new List<IUIObserver<StarsManager>>();

    public void AddObserver(IUIObserver<StarsManager> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IUIObserver<StarsManager> observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    public new void NotifyObservers(UIState state)
    {
        Debug.Log($"StarsManager: Notifying {observers.Count} observers of {state}");
        foreach (var observer in observers)
        {
            if (observer != null)
            {
                observer.OnStateChange(this, state);
            }
            else
            {
                Debug.LogWarning("Null observer found in StarsManager observers list!");
            }
        }
    }
}