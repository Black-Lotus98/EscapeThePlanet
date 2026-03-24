// Created by Qusai Fannoun
// This script must be attached to the fuel slider object in the scene editor
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelUIObserver : MonoBehaviour, IUIObserver<FuelManager>
{
    [Header("UI References")]
    [SerializeField] private Slider fuelSlider;
    
    private FuelManager fuelManager;
    private bool isRegistered = false;

    private void Awake()
    {
        // Validate UI components
        if (fuelSlider == null)
        {
            Debug.LogError("FuelSlider is not assigned in FuelUIObserver!");
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
        fuelManager = FindObjectOfType<FuelManager>();
        
        if (fuelManager == null)
        {
            Debug.LogError("FuelManager not found in scene!");
            enabled = false;
            return;
        }

        // Subscribe the observer to the fuel manager
        fuelManager.AddObserver(this);
        fuelManager.NotifyObservers(UIState.FuelChanged);
        isRegistered = true;
        
        // Debug.Log("FuelUIObserver registered successfully!");
    }

    public void OnStateChange(FuelManager fuelManager, UIState state)
    {
        if (state == UIState.FuelChanged)
        {
            UpdateFuelUI(fuelManager);
        }
    }

    private void UpdateFuelUI(FuelManager fuelManager)
    {
        if (fuelSlider == null || fuelManager == null)
        {
            Debug.LogWarning("FuelSlider or FuelManager is null in UpdateFuelUI!");
            return;
        }

        try
        {
            fuelSlider.maxValue = fuelManager.MaxFlightTime;
            fuelSlider.value = fuelManager.FuelAmount;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating fuel UI: {e.Message}");
        }
    }
}