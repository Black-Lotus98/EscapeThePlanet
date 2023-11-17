// Created by Qusai Fannoun
// this script must be attached to the shield slide object in the scene editor
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShieldUIObserver : MonoBehaviour,  IUIObserver<ShieldManager>
{
    private ShieldManager shieldManager;
    [SerializeField] Slider ShieldSlider;
    [SerializeField] TextMeshProUGUI ShieldText;

    private void Awake()
    {
        shieldManager = GameObject.FindObjectOfType<ShieldManager>();
        if (shieldManager != null)
        {
            // this will subscribe the observer to the player
            shieldManager.AddObserver(this);
            shieldManager.NotifyObservers(UIState.ShieldChanged);
        }
        else
        {
            Debug.LogError("Player not found.");
        }
    }

    public void OnStateChange(ShieldManager shieldManager, UIState state)
    {
        if (state == UIState.ShieldChanged)
        {
            // Debug.Log($"I am notified of {this} and I am Shield UI Observer.");
            UpdateShieldUi(shieldManager);
        }
    }

    private void UpdateShieldUi(ShieldManager shieldManager)
    {
        ShieldSlider.maxValue = shieldManager.ShieldMaxTime;
        ShieldSlider.value = shieldManager.CurrentShieldTime;
        ShieldText.text = (Mathf.Round(shieldManager.CurrentShieldTime * 100.00f) * 0.01f).ToString() + "/" + shieldManager.ShieldMaxTime.ToString();
    }


    // public void OnPlayerStateChange(Player player, UIState state)
    // {
    //     if (state == UIState.ShieldChanged)
    //     {
    //         Debug.Log($"I am notified of {this} and I am Shield UI Observer.");
    //         // UpdateShieldUi(player);
    //     }
    // }

    // the observer will be notified when the player state changes
    // the player state is not independent class so i must use Player.PlayerState
    // public void OnPlayerStateChange(Player player, Player.PlayerState state)
    // {
    //     if (state == Player.PlayerState.ShieldChanged)
    //     {
    //         // Debug.Log($"I am notified of {this} and I am Shield UI Observer.");

    //         UpdateShieldUi(player);
    //     }
    // }
    // private void UpdateShieldUi(Player player)
    // {
    //     ShieldSlider.maxValue = player.GetShieldMaxTime();
    //     ShieldSlider.value = player.GetCurrentShieldTime();
    //     ShieldText.text = (Mathf.Round(player.GetCurrentShieldTime() * 100.00f) * 0.01f).ToString() + "/" + player.GetShieldMaxTime().ToString();
    // }


}
