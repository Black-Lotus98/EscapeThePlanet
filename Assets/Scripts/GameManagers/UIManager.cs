using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// public class UIManager : Singleton<UIManager>, IUIObservable
public class UIManager : MonoBehaviour, IUIObservable<UIManager>
{
    public AudioSource AS;
    public AudioSource CollectableAS;
    public SaveDataManager saveDataManager;
    public InputHandler inputHandler;

    // Observers List
    private List<IUIObserver<UIManager>> observers = new List<IUIObserver<UIManager>>();
    public void AddObserver(IUIObserver<UIManager> observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IUIObserver<UIManager> observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers(UIState state)
    {
        foreach (var observer in observers)
        {
            observer.OnStateChange(this, state);
        }
    }


    void Start()
    {
        NotifyObservers(UIState.ShieldChanged);
        NotifyObservers(UIState.FuelChanged);
        NotifyObservers(UIState.StarsState);
        NotifyObservers(UIState.KeyState);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSources();
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {

            if (!inputHandler.GetComponent<InputHandler>().enabled)
            {
                AS.Stop();
            }
        }
    }

    protected LevelData GetLevelData(GameData gameData)
    {
        var sceneData = SceneManager.GetActiveScene();

        // using .buildIndex to get the scene index and .name to get the scene name
        var currentLevelData = gameData.levelData.Where(x => x.currentLevelIndex == sceneData.buildIndex).FirstOrDefault();
        if (currentLevelData == null)
        {
            currentLevelData = new LevelData(sceneData.name, sceneData.buildIndex);
        }
        return currentLevelData;
    }

    protected void FindSources()
    {

        var gameMaster = GameObject.FindGameObjectWithTag("GameMaster");
        if (gameMaster != null)
        {
            AS = gameMaster.gameObject.GetComponent<AudioSource>();
            CollectableAS = gameMaster.gameObject.GetComponent<AudioSource>();
            if (AS == null || CollectableAS == null)
            {
                Debug.LogError("AudioSource components not found on GameMaster.");
            }
        }
        else
        {
            Debug.LogError("GameMaster object not found.");
        }
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            inputHandler = GameObject.FindGameObjectWithTag("Player").gameObject.GetComponent<InputHandler>();
        }
        if (GameObject.Find("SaveDataManager") != null)
        {
            // saveDataManager = GameObject.Find("SaveDataManager").GetComponent<SaveDataManager>();

            saveDataManager = SaveDataManager.Instance;
        }
    }

}