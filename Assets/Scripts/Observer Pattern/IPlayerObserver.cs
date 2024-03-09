using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Currently not in use

// The Player observer interface
public interface IPlayerObserver
{
    // void OnPlayerStateChange(Player player, Player.PlayerState state);
    void OnPlayerStateChange(Player player, UIState state);
}
