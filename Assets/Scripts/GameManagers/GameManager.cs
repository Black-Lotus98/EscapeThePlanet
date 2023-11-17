using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool levelIsOver = false;
    public string nextLevel;
    public int levelToReach;
    public List<Star> starsList = new List<Star>();
    SaveDataManager saveManager;
    public bool LevelIsOver
    {
        get { return this.levelIsOver; }
        set { this.levelIsOver = value; }
    }

    private void Start()
    {
        saveManager = GameObject.Find("SaveDataManager").GetComponent<SaveDataManager>();

        var starts = GameObject.FindGameObjectsWithTag("StarCollectable");
        foreach (var star in starts)
        {
            starsList.Add(star.GetComponent<Star>());
        }

        var levelData = saveManager.Load().levelData.Where(x => x.currentLevelIndex == PlayerPrefs.GetInt("levelReached")).FirstOrDefault();
        // levelData.collectedStars = saveManager.GetCollectedStars();
        // Debug.Log($"Collected Stars: {saveManager.GetCollectedStars()}");

        GetComponent<SaveDataManager>();
        // Debug.Log($"Level Reached: {PlayerPrefs.GetInt("levelReached")}");
        LevelIsOver = false;
    }
    public void WinLevel()
    {
        LevelIsOver = true;
        if (PlayerPrefs.GetInt("levelReached") < levelToReach)
        {
            PlayerPrefs.SetInt("levelReached", levelToReach);
        }
    }

    public void RestartGame()
    {
        saveManager.ResetCollectedStars();

        PlayerPrefs.SetInt("levelReached", 1);
        LevelIsOver = false;
    }
}
