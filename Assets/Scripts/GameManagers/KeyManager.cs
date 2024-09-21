using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyManager : UIManager, IUIObservable<KeyManager>
{
    [Header("Key Settings")]
    [SerializeField] private bool playerHasKey = false;
    [SerializeField] private AudioClip keyCollectableSound;

    public bool PlayerHasKey
    {
        get { return playerHasKey; }
        set
        {
            if (playerHasKey == value)
            {
                return; // No change needed
            }

            playerHasKey = value;
            
            if (CollectableAS != null && keyCollectableSound != null)
            {
                CollectableAS.PlayOneShot(keyCollectableSound);
            }
            
            NotifyObservers(UIState.KeyState);
        }
    }

    private readonly List<IUIObserver<KeyManager>> observers = new List<IUIObserver<KeyManager>>();

    public void AddObserver(IUIObserver<KeyManager> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IUIObserver<KeyManager> observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    public new void NotifyObservers(UIState state)
    {
        Debug.Log($"KeyManager: Notifying {observers.Count} observers of {state}");
        foreach (var observer in observers)
        {
            if (observer != null)
            {
                observer.OnStateChange(this, state);
            }
            else
            {
                Debug.LogWarning("Null observer found in KeyManager observers list!");
            }
        }
    }
}
