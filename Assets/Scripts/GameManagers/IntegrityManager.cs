using System.Collections.Generic;
using UnityEngine;

public class IntegrityManager : UIManager, IUIObservable<IntegrityManager>
{
    private int currentIntegrity = -1;
    private int maxIntegrity = -1;

    public int CurrentIntegrity { get { return currentIntegrity; } }
    public int MaxIntegrity { get { return maxIntegrity; } }

    public void SetIntegrity(int current, int max)
    {
        currentIntegrity = current;
        maxIntegrity = max;
        NotifyObservers(UIState.IntegrityChanged);
    }

    private readonly List<IUIObserver<IntegrityManager>> observers = new List<IUIObserver<IntegrityManager>>();

    public void AddObserver(IUIObserver<IntegrityManager> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IUIObserver<IntegrityManager> observer)
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
                Debug.LogWarning("Null observer found in IntegrityManager observers list!");
            }
        }
    }
}
