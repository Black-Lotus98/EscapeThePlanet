using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyUIObserver : MonoBehaviour, IUIObserver<KeyManager>
{
    [Header("UI References")]
    [SerializeField] private Image keyImage;
    [SerializeField] private Sprite collectedKey;
    
    private KeyManager keyManager;
    private bool isRegistered = false;

    private void Awake()
    {
        // Validate UI components
        if (keyImage == null)
        {
            Debug.LogError("KeyImage is not assigned in KeyUIObserver!");
            enabled = false;
            return;
        }

        if (collectedKey == null)
        {
            Debug.LogError("CollectedKey sprite is not assigned in KeyUIObserver!");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        // Register with manager in Start() to ensure proper initialization order
        RegisterWithManager();
    }

    private void RegisterWithManager()
    {
        if (isRegistered) return;

        // Cache component reference for better performance
        keyManager = FindObjectOfType<KeyManager>();
        
        if (keyManager == null)
        {
            Debug.LogError("KeyManager not found in scene!");
            enabled = false;
            return;
        }

        // Subscribe the observer to the key manager
        keyManager.AddObserver(this);
        keyManager.NotifyObservers(UIState.KeyState);
        isRegistered = true;
        
        Debug.Log("KeyUIObserver registered successfully!");
    }

    public void OnStateChange(KeyManager keyManager, UIState state)
    {
        if (state == UIState.KeyState)
        {
            UpdateKeyUI(keyManager);
        }
    }

    private void UpdateKeyUI(KeyManager keyManager)
    {
        if (keyImage == null || keyManager == null)
        {
            Debug.LogWarning("KeyImage or KeyManager is null in UpdateKeyUI!");
            return;
        }

        try
        {
            if (keyManager.PlayerHasKey)
            {
                keyImage.sprite = collectedKey;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating key UI: {e.Message}");
        }
    }
}
