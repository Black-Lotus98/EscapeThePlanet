using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private string nextLevel;
    [SerializeField] private int levelToReach;
    
    [Header("References")]
    [SerializeField] private SaveDataManager saveManager;
    
    private bool levelIsOver = false;
    private readonly List<Star> starsList = new List<Star>();
    
    public bool LevelIsOver
    {
        get { return levelIsOver; }
        set { levelIsOver = value; }
    }

    private void Start()
    {
        // Get SaveDataManager instance
        saveManager = SaveDataManager.Instance;
        
        if (saveManager == null)
        {
            Debug.LogError("SaveDataManager instance not found!");
            return;
        }

        // Cache star objects once
        CacheStarObjects();
        
        // Load level data
        LoadLevelData();
        
        // Initialize level state
        LevelIsOver = false;
    }

    private void CacheStarObjects()
    {
        var starObjects = GameObject.FindGameObjectsWithTag("StarCollectable");
        
        if (starObjects.Length == 0)
        {
            // A level may legitimately have no star collectables.
            return;
        }
        
        foreach (var starObject in starObjects)
        {
            if (starObject != null)
            {
                var starComponent = starObject.GetComponent<Star>();
                if (starComponent != null)
                {
                    starsList.Add(starComponent);
                }
                else
                {
                    Debug.LogWarning($"Star component not found on {starObject.name}");
                }
            }
        }
        
        // Debug.Log($"Cached {starsList.Count} star objects.");
    }

    private void LoadLevelData()
    {
        try
        {
            // Level data is created on demand when a level is first played/saved,
            // so a missing entry here is normal (e.g. first run) and not an error.
            saveManager.Load();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading level data: {e.Message}");
        }
    }

    public void WinLevel()
    {
        LevelIsOver = true;
        
        var currentLevelReached = PlayerPrefs.GetInt("levelReached", 1);
        if (currentLevelReached < levelToReach)
        {
            PlayerPrefs.SetInt("levelReached", levelToReach);
        }
    }

    public void RestartGame()
    {
        if (saveManager != null)
        {
            saveManager.ResetCollectedStars();
        }
        
        PlayerPrefs.SetInt("levelReached", 1);
        LevelIsOver = false;
    }
}
