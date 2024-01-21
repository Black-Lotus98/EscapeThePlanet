using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SaveDataManager : Singleton<SaveDataManager>
{
    [SerializeField] GameData gameData;

    int currentCollectedStars = 0;
    int tempOldCollectedStars = 0;

    public int TempCollectedStars
    {
        get { return tempOldCollectedStars; }
        set
        {
            tempOldCollectedStars = value;
        }
    }
  

    public void ResetTempCollectedStars()
    {
        TempCollectedStars = 0;
    }


    public void Save(GameData data)
    {
        SaveManager.Save(data);
    }

    public GameData Load()
    {
        gameData = SaveManager.Load();
        if (gameData == null)
        {
            gameData = new GameData();
        }
        if (gameData.lastLevelReached < SceneManager.GetActiveScene().buildIndex)
        {
            gameData.lastLevelReached = SceneManager.GetActiveScene().buildIndex;
        }

        return gameData;
    }

    public void SaveCollectedStar()
    {
        if (currentCollectedStars < 3)
        {
            currentCollectedStars++;
        }
    }

    public int GetCollectedStars()
    {
        return currentCollectedStars;
    }

    public void ResetCollectedStars()
    {
        currentCollectedStars = 0;
    }

    public void Reset()
    {
        gameData = new GameData();
    }

    LevelData GetLevelData(GameData gameData)
    {
        var sceneData = SceneManager.GetActiveScene();

        // using .buildIndex to get the scene index and .name to get the scene name
        var currentLevelData = gameData.levelData.Where(x => x.currentLevelIndex == sceneData.buildIndex).FirstOrDefault();
        if (currentLevelData == null && sceneData.name.Contains("Level"))
        {
            currentLevelData = new LevelData(sceneData.name, sceneData.buildIndex, 0, 0);
        }

        return currentLevelData;
    }



}