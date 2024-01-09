using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PerformanceManager : MonoBehaviour
{
    SaveDataManager saveManager;

    // [SerializeField] GameObject StatsListContent;
    [SerializeField] GameObject levelStats;
    [SerializeField] Transform statsListContent;
    [SerializeField] TextMeshProUGUI noDataText;




    void Start()
    {
        saveManager = GameObject.Find("SaveDataManager").GetComponent<SaveDataManager>();
        GenerateStatsUI();
    }
    private void GenerateStatsUI()
    {
        // Load the saved game data
        GameData gameData = saveManager.Load();

        if (gameData == null || gameData.levelData.Count == 0)
        {
            // If no data is present, display a message
            noDataText.gameObject.SetActive(true);
            noDataText.text = "No saved data yet.";
        }
        else
        {
            // If data is present, instantiate the UI elements for each level's stats
            noDataText.gameObject.SetActive(false); // Hide the no data message
            foreach (LevelData levelData in gameData.levelData)
            {
                GameObject levelStatItem = Instantiate(levelStats, statsListContent);
                levelStatItem.transform.Find("LevelNameText").GetComponent<Text>().text = "Level " + levelData.currentLevelIndex + " Deaths: ";
                levelStatItem.transform.Find("DeathsText").GetComponent<Text>().text = levelData.numberOfDeaths.ToString();
                // ... set other stats as needed
            }
        }
    }
}
