using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour, IUIObservable<UIManager>
{
    [Header("Audio Sources")]
    [SerializeField] protected AudioSource AS;
    [SerializeField] protected AudioSource CollectableAS;
    
    [Header("References")]
    [SerializeField] protected SaveDataManager saveDataManager;
    [SerializeField] private InputHandler inputHandler;

    // Cached components for better performance
    private GameObject playerObject;
    private GameObject gameMasterObject;

    // Observers List
    private readonly List<IUIObserver<UIManager>> observers = new List<IUIObserver<UIManager>>();
    
    public void AddObserver(IUIObserver<UIManager> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IUIObserver<UIManager> observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    public void NotifyObservers(UIState state)
    {
        foreach (var observer in observers)
        {
            observer.OnStateChange(this, state);
        }
    }

    private void Start()
    {
        // Cache components once
        CacheComponents();
        
        // Initialize UI state
        NotifyObservers(UIState.ShieldChanged);
        NotifyObservers(UIState.FuelChanged);
        NotifyObservers(UIState.StarsState);
        NotifyObservers(UIState.KeyState);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSources();
    }

    private void Update()
    {
        // Only check if player exists and input handler is disabled
        if (playerObject != null && inputHandler != null && !inputHandler.enabled)
        {
            if (AS != null && AS.isPlaying)
            {
                AS.Stop();
            }
        }
    }

    private void CacheComponents()
    {
        // Cache player object once
        playerObject = GameObject.FindGameObjectWithTag("Player");
        
        // Cache game master object once
        gameMasterObject = GameObject.FindGameObjectWithTag("GameMaster");
        
        // Get SaveDataManager instance
        saveDataManager = SaveDataManager.Instance;
    }

    protected LevelData GetLevelData(GameData gameData)
    {
        var sceneData = SceneManager.GetActiveScene();

        // Using .buildIndex to get the scene index and .name to get the scene name
        var currentLevelData = gameData.levelData.Where(x => x.currentLevelIndex == sceneData.buildIndex).FirstOrDefault();
        if (currentLevelData == null)
        {
            currentLevelData = new LevelData(sceneData.name, sceneData.buildIndex);
        }
        return currentLevelData;
    }

    private void FindSources()
    {
        // Cache game master reference
        if (gameMasterObject == null)
        {
            gameMasterObject = GameObject.FindGameObjectWithTag("GameMaster");
        }
        
        if (gameMasterObject != null)
        {
            AudioSource[] sources = gameMasterObject.GetComponents<AudioSource>();
            if (sources.Length >= 2)
            {
                AS = sources[0];
                CollectableAS = sources[1];
            }
            else if (sources.Length == 1)
            {
                AS = sources[0];
                CollectableAS = sources[0];
            }
            else
            {
                Debug.LogError("GameMaster has no AudioSource components.");
            }
        }
        else
        {
            Debug.LogError("GameMaster object not found.");
        }
        
        // Cache player reference
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        
        if (playerObject != null)
        {
            inputHandler = playerObject.GetComponent<InputHandler>();
        }
        
        // Get SaveDataManager instance
        if (saveDataManager == null)
        {
            saveDataManager = SaveDataManager.Instance;
        }
    }
}