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
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (player != null)
        {
            player.AddObserver(this);
        }
        else
        {
            Debug.LogError("Player not found.");
        }
    }

    public void OnPlayerStateChange(Player player)
    {
        UpdateFuelUi(player);
    }

    private void UpdateFuelUi(Player player)
    {
        FuelSlider.maxValue = player.GetMaxFlightTime();
        FuelSlider.value = player.GetFuelCounter();
    }
}
