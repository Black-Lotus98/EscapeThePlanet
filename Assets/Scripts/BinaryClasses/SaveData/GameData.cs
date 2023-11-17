using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int totalNumberOfDeaths = 0;
    public int lastLevelReached = 1;

    public List<LevelData> levelData;

    public GameData()
    {
        this.totalNumberOfDeaths = 0;
        this.lastLevelReached = 1;
        this.levelData = new List<LevelData>();
    }
}
