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
        UpdateShieldUi(player);
    }

    private void UpdateShieldUi(Player player)
    {
        ShieldSlider.maxValue = player.GetShieldMaxTime();
        ShieldSlider.value = player.GetCurrentShieldTime();

        ShieldText.text = (Mathf.Round(player.GetCurrentShieldTime() * 100.00f) * 0.01f).ToString() + "/" + player.GetShieldMaxTime().ToString();
    }
}
