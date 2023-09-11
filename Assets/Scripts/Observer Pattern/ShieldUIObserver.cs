// Created by Qusai Fannoun
// this script must be attached to the shield slide object in the scene editor
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShieldUIObserver : MonoBehaviour, IPlayerObserver
{
    private Player player;
    [SerializeField] Slider ShieldSlider;
    [SerializeField] TextMeshProUGUI ShieldText;

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
        if (state == Player.PlayerState.ShieldChanged)
        {
            Debug.Log($"I am notified of {this} and I am Shield UI Observer.");

            UpdateShieldUi(player);
        }
    }
    private void UpdateShieldUi(Player player)
    {
        ShieldSlider.maxValue = player.GetShieldMaxTime();
        ShieldSlider.value = player.GetCurrentShieldTime();
        ShieldText.text = (Mathf.Round(player.GetCurrentShieldTime() * 100.00f) * 0.01f).ToString() + "/" + player.GetShieldMaxTime().ToString();
    }
}
