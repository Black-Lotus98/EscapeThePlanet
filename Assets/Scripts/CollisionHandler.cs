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

    SaveDataManager saveDataManager;
    InputHandler inputHandler;
    PlayerIntegrity integrity;
    ShieldManager shieldManager;
    bool searchedForShield;
    float hazardImmuneUntil;
    int lastHazardHitFrame = -1;

    AudioSource AS;


    bool isTransitioning = false;
    bool CollisionDisabled = false;

    static int numberOfDeaths = 0;

    // private CollisionState currentState;

    void Awake()
    {
        integrity = GetComponent<PlayerIntegrity>();
        if (integrity == null)
        {
            integrity = gameObject.AddComponent<PlayerIntegrity>();
        }
    }

    void Start()
    {
        // Finding the SaveDataManager

        // This will look for the save data manager in the scene, but since we are using the singleton pattern,
        // we don't need it because we can access it globally
        // SaveManager = GameObject.Find("SaveDataManager").GetComponent<SaveDataManager>();

        // However, this way we dont need to used SaveDataManager.Instance every time we need it,
        // we just use the word saveDataManager
        saveDataManager = SaveDataManager.Instance;
        inputHandler = GetComponent<InputHandler>();
        shieldManager = FindObjectOfType<ShieldManager>();

        // Loading the data from the SaveDataManager
        GameData gameData = saveDataManager.Load();
        if (gameData == null)
            gameData = new GameData();

        // GameData gameData = SaveManager.Load();
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
            saveDataManager.Save(gameData);

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
            return;
        }

        if (IsHazard(other.gameObject))
        {
            if (ShieldIsBlocking())
            {
                if (other.gameObject.CompareTag("Bullet")) Destroy(other.gameObject);
                return;
            }
            TakeHazardHit();
            return;
        }

        Debug.Log($"{other.gameObject.name} has no CollisionState");
        StartCrashSequence();

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

    bool ShieldIsBlocking()
    {
        if (shieldManager == null && !searchedForShield)
        {
            searchedForShield = true;
            shieldManager = FindObjectOfType<ShieldManager>();
        }
        return shieldManager != null && shieldManager.ShieldIsActive;
    }

    static bool IsHazard(GameObject other)
    {
        return other.CompareTag("Bullet")
            || other.GetComponentInParent<FollowingEnemy>() != null
            || other.GetComponentInParent<MovingEnemy>() != null;
    }

    public void TakeHazardHit()
    {
        if (isTransitioning || CollisionDisabled) return;
        if (isTutorial)
        {
            StartCrashSequence();
            return;
        }
        if (Time.frameCount == lastHazardHitFrame) return;
        if (Time.time < hazardImmuneUntil) return;
        if (ShieldIsBlocking()) return;

        lastHazardHitFrame = Time.frameCount;

        if (integrity == null || !integrity.Absorb())
        {
            StartCrashSequence();
            return;
        }

        hazardImmuneUntil = Time.time + DifficultyManager.HitImmunityFor(DifficultyManager.Current);
        PlayHazardHitFeedback();
    }

    void PlayHazardHitFeedback()
    {
        if (AS != null && Explosion != null)
        {
            AS.PlayOneShot(Explosion, 0.5f);
        }
        if (ExplosionParticles != null)
        {
            CancelInvoke(nameof(StopHazardHitParticles));
            ExplosionParticles.Play();
            Invoke(nameof(StopHazardHitParticles), 0.25f);
        }
    }

    void StopHazardHitParticles()
    {
        if (ExplosionParticles != null)
        {
            ExplosionParticles.Stop();
        }
    }

    public void StartCrashSequence()
    {

        isTransitioning = true;
        CancelInvoke(nameof(StopHazardHitParticles));
        AS.Stop();
        AS.PlayOneShot(Explosion);
        ExplosionParticles.Play();
        if (inputHandler != null) inputHandler.enabled = false;

        if (isTutorial)
        {
            // Tutorials are exempt from checkpoints/stats: just reload.
            Invoke("ReloadLevel", delayTime);
            return;
        }

        if (CheckpointManager.Instance == null)
        {
            // Non-checkpoint level: a death counts and reloads from the start.
            RegisterDeath();
            Invoke("ReloadLevel", delayTime);
            return;
        }

        // Checkpoint level: defer to the respawn flow. A checkpoint respawn (revive
        // attempt) does NOT count as a death; only a true game-over does (see DoRespawn).
        Invoke(nameof(DoRespawn), delayTime);
    }

    void DoRespawn()
    {
        bool respawned = CheckpointManager.Instance.OnPlayerDied();
        if (respawned)
        {
            // Attempt consumed - NOT counted as a death.
            ExplosionParticles.Stop();
            if (inputHandler != null) inputHandler.enabled = true;
            isTransitioning = false;
            CollisionDisabled = false;
            hazardImmuneUntil = 0f;
        }
        else
        {
            // Out of revives: the run truly failed -> count one death. CheckpointManager
            // has shown the Game-Over panel (or restarted the level via fallback).
            RegisterDeath();
        }
    }

    // Persists a single death to the stats (per-level + total + saved counter).
    void RegisterDeath()
    {
        numberOfDeaths++;
        UpdateDataOnLosing();
        SaveProgress();
    }

    public void StartSuccessSequence()
    {
        GameData gameData = saveDataManager.Load();
        LevelData currentLevelData = GetLevelData(gameData);
        int currentCollectedStars = saveDataManager.GetCollectedStars();
        Debug.Log($"Current Collected Stars inside StartSuccessSequence: {currentCollectedStars}");
        if (saveDataManager.TempCollectedStars >= currentCollectedStars)
        {
            currentLevelData.collectedStars = saveDataManager.TempCollectedStars;
            Debug.Log($"Current Collected Stars inside if statement condition 1: {currentCollectedStars}");
        }
        else
        {
            currentLevelData.collectedStars = currentCollectedStars;
            Debug.Log($"Current Collected Stars inside if statement condition 2: {currentCollectedStars}");

        }

        saveDataManager.ResetCollectedStars();
        saveDataManager.ResetTempCollectedStars();
        isTransitioning = true;
        AS.Stop();
        AS.PlayOneShot(Finish);
        FinishParticles.Play();
        if (inputHandler != null) inputHandler.enabled = false;
        gameManager.WinLevel();
        Invoke("LoadNextLevel", delayTime);
        saveDataManager.Save(gameData);
    }
    public void FinishTutorial()
    {
        isTransitioning = true;
        AS.Stop();
        AS.PlayOneShot(Finish);
        FinishParticles.Play();
        if (inputHandler != null) inputHandler.enabled = false;
        FinishTutorialPlane.SetActive(true);
        Invoke("loadTutorial", delayTime);

    }

    public void ActivateCheckpoint(Transform checkpointTransform)
    {
        if (isTutorial) return;
        if (CheckpointManager.Instance != null && checkpointTransform != null)
        {
            CheckpointManager.Instance.ActivateCheckpoint(checkpointTransform, checkpointTransform.gameObject);
        }
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
        // No save file yet (first run) returns null — keep the current count.
        if (data != null)
        {
            numberOfDeaths = data.NumberOfDeaths;
        }
    }


    void UpdateDataOnLosing()
    {
        GameData gameData = saveDataManager.Load();
        var currentLevelData = GetLevelData(gameData);
        currentLevelData.numberOfDeaths++;
        saveDataManager.ResetTempCollectedStars();
        gameData.totalNumberOfDeaths++;
        saveDataManager.Save(gameData);
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
            currentLevelData = new LevelData(sceneData.name, sceneData.buildIndex);
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
