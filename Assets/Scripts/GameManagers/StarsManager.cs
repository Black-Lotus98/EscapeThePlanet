using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarsManager : UIManager, IUIObservable<StarsManager>, ICheckpointable
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

    // Memento Pattern: snapshot/restore collected-star count at a checkpoint. Captures
    // both the in-scene counter and the SaveDataManager running total, and restores them
    // to absolute values so a respawn never double-counts.
    private class StarsMemento
    {
        public int counter;
        public int savedTotal;
    }

    public object CaptureState()
    {
        SaveDataManager sdm = saveDataManager != null ? saveDataManager : SaveDataManager.Instance;
        return new StarsMemento
        {
            counter = collectedStarsCounter,
            savedTotal = sdm != null ? sdm.GetCollectedStars() : 0,
        };
    }

    public void RestoreState(object memento)
    {
        if (memento is StarsMemento starsMemento)
        {
            collectedStarsCounter = starsMemento.counter;
            SaveDataManager sdm = saveDataManager != null ? saveDataManager : SaveDataManager.Instance;
            if (sdm != null)
            {
                sdm.SetCollectedStars(starsMemento.savedTotal);
            }
            NotifyObservers(UIState.StarsState);
        }
    }
}