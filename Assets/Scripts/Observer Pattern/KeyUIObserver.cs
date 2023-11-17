using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyUIObserver : MonoBehaviour, IUIObserver<KeyManager>
{

    private KeyManager keyManager;
    [SerializeField] Image KeyImage;
    [SerializeField] Sprite CollectedKey;
    [SerializeField] Sprite DamagedKey;
    private void Awake()
    {
        keyManager = GameObject.FindObjectOfType<KeyManager>();
        if (keyManager != null)
        {
            // this will subscribe the observer to the player
            keyManager.AddObserver(this);
            keyManager.NotifyObservers(UIState.KeyState);
        }
        else
        {
            Debug.LogError("Player not found.");
        }
    }

    public void OnStateChange(KeyManager KeyManager, UIState state)
    {
        if (state == UIState.KeyState)
        {
            // Debug.Log($"I am notified of {this} and I am Fuel UI Observer.");
            UpdateKeyUI();
        }
    }

    private void UpdateKeyUI()
    {
        KeyImage.GetComponent<Image>().sprite = CollectedKey;
    }
}
