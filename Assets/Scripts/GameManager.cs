using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool levelIsOver = false;
    public string nextLevel;
    public int levelToReach;

    public bool LevelIsOver
    {
        get
        {
            return this.levelIsOver;
        }
        private set
        {
            this.levelIsOver = value;
        }
    }

    private void Start()
    {
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

        PlayerPrefs.SetInt("levelReached", 1);
        LevelIsOver = false;
    }
}
