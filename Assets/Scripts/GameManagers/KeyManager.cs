using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyManager : UIManager, IUIObservable<KeyManager>
{
    [SerializeField] bool playerHasKey = false;
    [SerializeField] AudioClip keyCollectableSound;

    public bool PlayerHasKey
    {
        get { return this.playerHasKey; }
        set
        {
            playerHasKey = value;
            CollectableAS.PlayOneShot(keyCollectableSound);
            NotifyObservers(UIState.KeyState);
        }
    }
    private List<IUIObserver<KeyManager>> observers = new List<IUIObserver<KeyManager>>();

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
        foreach (var observer in observers)
        {
            observer.OnStateChange(this, state);
        }
    }


}
