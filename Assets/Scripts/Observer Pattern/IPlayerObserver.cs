using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerObserver
{
    void OnPlayerStateChange(Player player);
}
