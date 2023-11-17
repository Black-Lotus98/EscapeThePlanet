using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;


public class LevelSelector : MonoBehaviour
{
    [SerializeField] Button[] levelButtons;

    [SerializeField] GameObject loadingScreen;
    [SerializeField] Slider loadingBarFill;
    [SerializeField] TextMeshProUGUI LoadingText;

    [SerializeField] Image[][] levelStars;

    [SerializeField] Sprite collectedStar;
    [SerializeField] Sprite notCollectedStar;



    void Start()
    {
        int levelReached = PlayerPrefs.GetInt("levelReached", 1);
        // GameData gameData = SaveManager.Load();

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                levelButtons[i].interactable = false;
            }
        }
        InitializeLevelStars();
    }


    void InitializeLevelStars()
    {
        levelStars = new Image[levelButtons.Length][];

        GameData gameData = SaveManager.Load();
        if (gameData == null)
        {
            gameData = new GameData();
        }
        // I have used PlayerPrefs to store the level reached instead of altering the whole GameData class
        var lastLevelReached = PlayerPrefs.GetInt("levelReached", 1);

        for (int i = 0; i < lastLevelReached; i++)
        {
            Transform starPanel = levelButtons[i].transform.Find("Text/StarsPanel");

            // The(i+1) is to get the correct level index 
            var currentLevelData = gameData.levelData.Where(x => x.currentLevelIndex == i + 1).FirstOrDefault();
            if (currentLevelData == null)
            {
                currentLevelData = new LevelData("Level" + (i + 1), i + 1, 0, 0);
            }
            if (starPanel != null)
            {
                levelStars[i] = new Image[3];
                for (int j = 0; j < 3; j++)
                {
                    string starName = $"Star{j + 1}";
                    Image starImage = starPanel.Find(starName)?.GetComponent<Image>();

                    if (starImage != null)
                    {
                        levelStars[i][j] = starImage;
                        if (j < currentLevelData.collectedStars)
                        {
                            levelStars[i][j].sprite = collectedStar;
                        }

                        // if (i + 1 < 10)
                        // {
                        //     currentLevelName = $"Level0{i + 1}";
                        // }
                        // else
                        // {
                        //     currentLevelName = $"Level{i + 1}";
                        // }
                        // if (i + 1 < PlayerPrefs.GetInt("levelReached", 1))
                        // {
                        //     LevelData currentLevelData = gameData.levels[currentLevelName];


                        //     
                        // }
                        // else
                        // {
                        //     levelStars[i][j].sprite = notCollectedStar;
                        // }
                    }
                    else
                    {
                        Debug.LogWarning($"Star image not found for level {i} star {j}");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"StarsPanel not found in level button {i}");
            }
        }
    }


    public void LoadLevel(string lvlName)
    {
        StartCoroutine(LoadSceneAsync(lvlName));
        // SceneManager.LoadScene(lvlName);`
    }

    IEnumerator LoadSceneAsync(string lvlName)
    {
        loadingScreen.SetActive(true);

        AsyncOperation loadingProcess = SceneManager.LoadSceneAsync(lvlName);


        while (!loadingProcess.isDone)
        {
            float progressValue = Mathf.Clamp01(loadingProcess.progress / 0.9f);

            loadingBarFill.value = progressValue;
            LoadingText.text = "% " + Mathf.RoundToInt(progressValue * 100f).ToString();

            yield return null;
        }

    }
}
