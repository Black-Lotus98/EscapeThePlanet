using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class PerformanceManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SaveDataManager saveManager;
    [SerializeField] private GameObject levelStats;
    [SerializeField] private Transform statsListContent;
    [SerializeField] private TextMeshProUGUI noDataText;

    private void Start()
    {
        // Get SaveDataManager instance
        saveManager = SaveDataManager.Instance;
        
        if (saveManager == null)
        {
            Debug.LogError("SaveDataManager instance not found!");
            return;
        }
        
        GenerateStatsUI();
    }
    
    private void GenerateStatsUI()
    {
        if (noDataText == null)
        {
            Debug.LogError("NoDataText is not assigned!");
            return;
        }

        // Load the saved game data
        GameData gameData = saveManager.Load();

        if (gameData == null || gameData.levelData == null || gameData.levelData.Count == 0)
        {
            // If no data is present, display a message
            noDataText.gameObject.SetActive(true);
            noDataText.text = "No saved data yet.";
            return;
        }

        // If data is present, instantiate the UI elements for each level's stats
        noDataText.gameObject.SetActive(false); // Hide the no data message
        
        if (levelStats == null || statsListContent == null)
        {
            Debug.LogError("LevelStats or StatsListContent is not assigned!");
            return;
        }
        
        foreach (LevelData levelData in gameData.levelData)
        {
            if (levelData == null) continue;
            
            GameObject levelStatItem = Instantiate(levelStats, statsListContent);
            
            // Find and update level name text
            var levelNameText = levelStatItem.transform.Find("LevelNameText")?.GetComponent<Text>();
            if (levelNameText != null)
            {
                levelNameText.text = $"Level {levelData.currentLevelIndex} Deaths: ";
            }
            
            // Find and update deaths text
            var deathsText = levelStatItem.transform.Find("DeathsText")?.GetComponent<Text>();
            if (deathsText != null)
            {
                deathsText.text = levelData.numberOfDeaths.ToString();
            }
        }
    }
}
