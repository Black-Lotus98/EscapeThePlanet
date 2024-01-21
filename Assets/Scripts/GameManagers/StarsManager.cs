using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarsManager : UIManager, IUIObservable<StarsManager>
{
    int collectedStarsCounter = 0;
    [SerializeField] AudioClip starCollectableSound;


    void Start()
    {
        LevelData currentLevelData = GetLevelData(saveDataManager.Load());
        saveDataManager.TempCollectedStars = currentLevelData.collectedStars;
        currentLevelData.collectedStars = 0;

    }

    public int CollectedStarsCounter
    {
        get { return collectedStarsCounter; }
        set
        {
            if (collectedStarsCounter < 3)
            {
                LevelData currentLevelData = GetLevelData(saveDataManager.Load());
                collectedStarsCounter += value;
                saveDataManager.SaveCollectedStar();
                AS.PlayOneShot(starCollectableSound);
                NotifyObservers(UIState.StarsState);

            }
        }
    }
    List<IUIObserver<StarsManager>> observers = new List<IUIObserver<StarsManager>>();

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
            observer.OnStateChange(this, state);
        }
    }

}