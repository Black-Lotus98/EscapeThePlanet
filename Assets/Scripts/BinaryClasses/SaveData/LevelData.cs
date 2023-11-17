
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string currentLevelName = "Level01";
    public int currentLevelIndex = 1;
    public int collectedStars = 0;
    public int numberOfDeaths = 0;
    // public Checkpoint lastCheckpoint;


    public LevelData(string aLevelName, int aLevelIndex, int aCollectedStars, int aNumberOfDeaths)
    {
        this.currentLevelName = aLevelName;
        this.currentLevelIndex = aLevelIndex;
        this.collectedStars = aCollectedStars;
        this.numberOfDeaths = aNumberOfDeaths;
    }



}



