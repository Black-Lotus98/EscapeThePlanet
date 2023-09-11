// Created by Qusai Fannoun
// this script must be attached to the fuel slide object in the scene editor
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelUIObserver : MonoBehaviour, IPlayerObserver
{
    private Player player;
    [SerializeField] Slider FuelSlider;

    private void Awake()
    {
        player = GameObject.FindObjectOfType<Player>();
        if (player != null)
        {
            // this will subscribe the observer to the player
            player.AddObserver(this);
        }
        else
        {
            Debug.LogError("Player not found.");
        }
    }

    // the observer will be notified when the player state changes
    // the player state is not independent class so i must use Player.PlayerState
    public void OnPlayerStateChange(Player player, Player.PlayerState state)
    {
        if (state == Player.PlayerState.FuelChanged)
        {
            Debug.Log($"I am notified of {this} and I am Fuel UI Observer.");
            UpdateFuelUi(player);
        }
    }
    private void UpdateFuelUi(Player player)
    {
        // Debug.Log($"I am notified of {this} and I am Update Fuel UI Observer.");
        FuelSlider.maxValue = player.GetMaxFlightTime();
        FuelSlider.value = player.GetFuelCounter();
    }
}
