using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{

    [SerializeField] GameManager gameManager;
    [SerializeField] float delayTime = 1f;
    [SerializeField] AudioClip Explosion;
    [SerializeField] AudioClip Finish;

    [SerializeField] ParticleSystem ExplosionParticles;
    [SerializeField] ParticleSystem FinishParticles;
    [SerializeField] bool isTutorial = false;
    [SerializeField] GameObject FinishTutorialPlane;

    SaveDataManager SaveManager;


    AudioSource AS;


    bool isTransitioning = false;
    bool CollisionDisabled = false;

    static int numberOfDeaths = 0;

    // private CollisionState currentState;

    void Start()
    {
        // Finding the SaveDataManager

        SaveManager = GameObject.Find("SaveDataManager").GetComponent<SaveDataManager>();

        // Loading the data from the SaveDataManager
        GameData gameData = SaveManager.Load();
        // Getting the current level data from the game data
        // LevelData currentLevelData = GetLevelData(gameData);

        var currentSceneData = SceneManager.GetActiveScene();
        if (!gameData.levelData.Where(x => x.currentLevelName == currentSceneData.name).Any())
        {
            gameData.levelData.Add(new LevelData(currentSceneData.name, currentSceneData.buildIndex, 0, 0));
        }

        if (gameData.lastLevelReached < SceneManager.GetActiveScene().buildIndex)
        {
            gameData.lastLevelReached = SceneManager.GetActiveScene().buildIndex;
        }
        AS = GetComponent<AudioSource>();

        if (!isTutorial)
        {
            SaveManager.Save(gameData);

            //Load the data to check if the game is restarted or not
            LoadData();
            //Then the data is saved again to keep it on track
            SaveProgress();
        }

    }

    // void Update()
    // {
    //     ActivateController();
    //     CheatKeys();
    // }

    void OnCollisionEnter(Collision other)
    {
        if (isTransitioning || CollisionDisabled)
        {
            return;
        }

        CollisionState state = other.gameObject.GetComponent<CollisionState>();
        if (state != null)
        {
            state.Handle(this);
        }
        else
        {
            Debug.Log($"{other.gameObject.name} has no CollisionState");
            StartCrashSequence();
        }

        // the switch statement is not needed since we have a CollisionState that being attached to game objects
        // switch (other.gameObject.tag)
        // {
        //     case "Start":
        //         currentState = new StartState();
        //         // Debug.Log("You hit the Start pad, I am in collision handler");
        //         break;
        //     case "FuelPad":
        //         //Debug.Log("You hit the Fuel pad");
        //         currentState = new FuelPadState();
        //         break;
        //     case "Finish":
        //         StartSuccessSequence();
        //         break;
        //     default:
        //         StartCrashSequence();
        //         break;
        // }
        // currentState.Handle(this);

    }

    public void StartCrashSequence()
    {

        isTransitioning = true;
        AS.Stop();
        AS.PlayOneShot(Explosion);
        ExplosionParticles.Play();
        GetComponent<InputHandler>().enabled = false;
        numberOfDeaths++;
        if (!isTutorial)
        {
            UpdateDataOnLosing();
            SaveProgress();
        }
        Invoke("ReloadLevel", delayTime);
    }

    public void StartSuccessSequence()
    {
        GameData gameData = SaveManager.Load();
        LevelData currentLevelData = GetLevelData(gameData);
        int currentCollectedStars = SaveManager.GetCollectedStars();
        if (SaveManager.TempCollectedStars >= currentCollectedStars)
        {
            currentLevelData.collectedStars = SaveManager.TempCollectedStars;
        }
        else
        {
            currentLevelData.collectedStars = currentCollectedStars;
        }

        SaveManager.ResetCollectedStars();
        SaveManager.ResetTempCollectedStars();
        isTransitioning = true;
        AS.Stop();
        AS.PlayOneShot(Finish);
        FinishParticles.Play();
        GetComponent<InputHandler>().enabled = false;
        gameManager.WinLevel();
        Invoke("LoadNextLevel", delayTime);
        SaveManager.Save(gameData);
    }
    public void FinishTutorial()
    {
        isTransitioning = true;
        AS.Stop();
        AS.PlayOneShot(Finish);
        FinishParticles.Play();
        GetComponent<InputHandler>().enabled = false;
        FinishTutorialPlane.SetActive(true);
        Invoke("loadTutorial", delayTime);

    }

    public void ActivateCheckpoint()
    {
        Debug.Log($"Checkpoint activated");
    }


    void ReloadLevel()
    {
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(CurrentSceneIndex);
    }
    public int getNumberOfDeaths()
    {
        return numberOfDeaths;
    }
    public void setNumberOfDeaths()
    {
        numberOfDeaths = 0;
    }

    public void SaveProgress()
    {
        SaveData.SavePlayerData(this);
    }

    public void LoadData()
    {
        PlayerData data = SaveData.LoadPlayer();
        numberOfDeaths = data.NumberOfDeaths;
    }


    void UpdateDataOnLosing()
    {
        GameData gameData = SaveManager.Load();
        var currentLevelData = GetLevelData(gameData);
        currentLevelData.numberOfDeaths++;
        SaveManager.ResetTempCollectedStars();
        gameData.totalNumberOfDeaths++;
        SaveManager.Save(gameData);
    }

    // The method is used in the StartSuccessSequence method
    void LoadNextLevel()
    {
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int NextSceneIndex = CurrentSceneIndex + 1;

        string CurrentSceneName = SceneManager.GetSceneByBuildIndex(CurrentSceneIndex).name;

        if ((NextSceneIndex == SceneManager.sceneCountInBuildSettings) || (CurrentSceneName == "LastLevel"))
        {
            NextSceneIndex = 0;
        }
        SceneManager.LoadScene(NextSceneIndex);
    }



    LevelData GetLevelData(GameData gameData)
    {
        var sceneData = SceneManager.GetActiveScene();

        // using .buildIndex to get the scene index and .name to get the scene name
        var currentLevelData = gameData.levelData.Where(x => x.currentLevelIndex == sceneData.buildIndex).FirstOrDefault();
        if (currentLevelData == null)
        {
            currentLevelData = new LevelData(sceneData.name, sceneData.buildIndex, 1, 0);
        }
        return currentLevelData;
    }

    void loadTutorial()
    {
        int CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (CurrentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(CurrentSceneIndex + 1);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    // This method is used only on the PC platform
    // void CheatKeys()
    // {
    //     if (Input.GetKeyDown(KeyCode.L))
    //     {
    //         LoadNextLevel();
    //     }

    //     else if (Input.GetKeyDown(KeyCode.C))
    //     {
    //         CollisionDisabled = !CollisionDisabled;
    //     }
    //     else if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         ReloadLevel();
    //     }

    // }


    // void ActivateController()
    // {
    //     if(GameObject.FindGameObjectWithTag("AnimatedCamera")==null && isTransitioning != true)
    //     {
    //         gameObject.GetComponent<MobileController>().enabled=true;
    //     }
    //     else
    //     {
    //         gameObject.GetComponent<MobileController>().enabled=false;
    //     }
    // }
}
