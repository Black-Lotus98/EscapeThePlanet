using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


[System.Serializable]
public class PlayerData
{
    public int NumberOfDeaths;
    public int LastLevelReached;


    public PlayerData(CollisionHandler collisionHandler)
    {
        this.NumberOfDeaths = collisionHandler.getNumberOfDeaths();
        this.LastLevelReached = PlayerPrefs.GetInt("levelReached");
    }

    public PlayerData()
    {
        this.NumberOfDeaths=0;
        this.LastLevelReached= 1;
    }
}
