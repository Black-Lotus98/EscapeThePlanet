// Created by Qusai Fannoun
// This script must be attached to the shield slider object in the scene editor
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShieldUIObserver : MonoBehaviour, IUIObserver<ShieldManager>
{
    [Header("UI References")]
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private TextMeshProUGUI shieldText;
    
    private ShieldManager shieldManager;
    private bool isRegistered = false;

    private void Awake()
    {
        // Validate UI components
        if (shieldSlider == null)
        {
            Debug.LogError("ShieldSlider is not assigned in ShieldUIObserver!");
            enabled = false;
            return;
        }

        if (shieldText == null)
        {
            Debug.LogError("ShieldText is not assigned in ShieldUIObserver!");
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
        shieldManager = FindObjectOfType<ShieldManager>();
        
        if (shieldManager == null)
        {
            Debug.LogError("ShieldManager not found in scene!");
            enabled = false;
            return;
        }

        // Subscribe the observer to the shield manager
        shieldManager.AddObserver(this);
        shieldManager.NotifyObservers(UIState.ShieldChanged);
        isRegistered = true;
        
        Debug.Log("ShieldUIObserver registered successfully!");
    }

    public void OnStateChange(ShieldManager shieldManager, UIState state)
    {
        if (state == UIState.ShieldChanged)
        {
            UpdateShieldUI(shieldManager);
        }
    }

    private void UpdateShieldUI(ShieldManager shieldManager)
    {
        if (shieldSlider == null || shieldText == null || shieldManager == null)
        {
            Debug.LogWarning("UI components or ShieldManager is null in UpdateShieldUI!");
            return;
        }

        try
        {
            shieldSlider.maxValue = shieldManager.ShieldMaxTime;
            shieldSlider.value = shieldManager.CurrentShieldTime;
            
            // Format shield time display with proper rounding
            float roundedTime = Mathf.Round(shieldManager.CurrentShieldTime * 100.0f) * 0.01f;
            shieldText.text = $"{roundedTime:F2}/{shieldManager.ShieldMaxTime:F2}";
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating shield UI: {e.Message}");
        }
    }
}
