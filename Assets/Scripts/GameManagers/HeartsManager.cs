using System.Collections.Generic;
using UnityEngine;

// Observable HUD bridge for the revive-attempts ("hearts") display. Mirrors the
// existing manager pattern (KeyManager/StarsManager): inherits UIManager and keeps its
// own typed observer list. The attempts LOGIC lives in CheckpointManager, which pushes
// values here via SetHearts. A negative max means "unlimited" (Easy) -> HUD hides.
public class HeartsManager : UIManager, IUIObservable<HeartsManager>
{
    private int currentHearts = -1;
    private int maxHearts = -1;

    public int CurrentHearts { get { return currentHearts; } }
    public int MaxHearts { get { return maxHearts; } }
    public bool IsUnlimited { get { return maxHearts < 0; } }

    public void SetHearts(int current, int max)
    {
        currentHearts = current;
        maxHearts = max;
        NotifyObservers(UIState.HeartsChanged);
    }

    private readonly List<IUIObserver<HeartsManager>> observers = new List<IUIObserver<HeartsManager>>();

    public void AddObserver(IUIObserver<HeartsManager> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IUIObserver<HeartsManager> observer)
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
                Debug.LogWarning("Null observer found in HeartsManager observers list!");
            }
        }
    }
}
