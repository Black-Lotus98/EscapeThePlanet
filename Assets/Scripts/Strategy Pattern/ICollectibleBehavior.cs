using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectibleBehavior<T> where T : UIManager
{
    // void ExecutePowerUp(Player player);  
    void ExecutePowerUp(T manager);

}
