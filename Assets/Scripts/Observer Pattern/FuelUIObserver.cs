// Created by Qusai Fannoun
// this script must be attached to the fuel slide object in the scene editor
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelUIObserver : MonoBehaviour, IUIObserver<FuelManager>
{
    private FuelManager fuelManager;
    [SerializeField] Slider FuelSlider;

    private void Awake()
    {
        fuelManager = GameObject.FindObjectOfType<FuelManager>();
        if (fuelManager != null)
        {
            // this will subscribe the observer to the player
            fuelManager.AddObserver(this);
            fuelManager.NotifyObservers(UIState.FuelChanged);
        }
        else
        {
            Debug.LogError("Player not found.");
        }
    }

    // the observer will be notified when the player state changes
    // the player state is not independent class so i must use Player.PlayerState
    // public void OnPlayerStateChange(Player player, Player.PlayerState state)
    // {
    //     if (state == Player.PlayerState.FuelChanged)
    //     {
    //         // Debug.Log($"I am notified of {this} and I am Fuel UI Observer.");
    //         UpdateFuelUi(player);
    //     }
    // }

    public void OnStateChange(FuelManager fuelManager, UIState state)
    {
        if (state == UIState.FuelChanged)
        {
            // Debug.Log($"I am notified of {this} and I am Fuel UI Observer.");
            UpdateFuelUi(fuelManager);
        }
    }
    private void UpdateFuelUi(FuelManager fuelManager)
    {
        // Debug.Log($"I am notified of {this} and I am Update Fuel UI Observer.");
        FuelSlider.maxValue = fuelManager.MaxFlightTime;
        FuelSlider.value = fuelManager.FuelAmount;
    }
}
