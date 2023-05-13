using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    public GameManager gameManager;

    static int numberOfDeaths = 0;
    [SerializeField] float delayTime = 1f;
    [SerializeField] AudioClip Explosion;
    [SerializeField] AudioClip Finish;

    [SerializeField] ParticleSystem ExplosionParticles;
    [SerializeField] ParticleSystem FinishParticles;
    [SerializeField] bool isTutorial = false;
    [SerializeField] GameObject FinishTutorialPlane;


    AudioSource AS;


    bool isTransitioning = false;
    bool CollisionDisabled = false;

    void Start()
    {
        AS = GetComponent<AudioSource>();

        //Load the data to check if the game is restarted or not
        LoadData();
        //Then the data is saved again to keep it on track
        SaveProgress();

    }

    void Update()
    {
        // ActivateController();
        //Cheatkeys();
    }
    void OnCollisionEnter(Collision other)
    {
        if (isTransitioning || CollisionDisabled)
        {
            return;
        }
        switch (other.gameObject.tag)
        {
            case "Start":
                //Debug.Log("You hit the lanch padl");
                break;
            case "FuelPad":
                //Debug.Log("You hit the Fuel pad");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartCrashSequence();
                break;
        }
    }

    void StartCrashSequence()
    {
        isTransitioning = true;
        AS.Stop();
        AS.PlayOneShot(Explosion);
        ExplosionParticles.Play();
        GetComponent<InputHandler>().enabled = false;
        numberOfDeaths++;
        SaveProgress();
        Invoke("ReloadLevel", delayTime);
    }

    void StartSuccessSequence()
    {
        isTransitioning = true;
        AS.Stop();
        AS.PlayOneShot(Finish);
        FinishParticles.Play();
        GetComponent<InputHandler>().enabled = false;
        if (!isTutorial)
        {
            gameManager.WinLevel();
            Invoke("LoadNextLevel", delayTime);
        }
        else
        {
            FinishTutorialPlane.SetActive(true);
            Invoke("loadTutorial", delayTime);
        }
    }


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


    // //This method is used only if the game was built on PC platform
    // void Cheatkeys()
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


}
