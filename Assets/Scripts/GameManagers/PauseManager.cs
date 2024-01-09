using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;


public class PauseManager : MonoBehaviour
{
    public bool IsPaused;

    public GameObject PauseMenuObj;
    public GameObject HowToPlayPanel;
    bool toggle = true;




    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Unpause();
        }
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReturnToGame()
    {
        Unpause();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || CrossPlatformInputManager.GetButtonDown("Pause"))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (IsPaused)
        {
            Unpause();
        }
        else
        {
            Pause();
        }
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        PauseMenuObj.SetActive(false);
        HowToPlayPanel.SetActive(false);

        IsPaused = false;
        if (toggle)
        {
            AudioListener.volume = 1f;
            toggle = !toggle;
        }
    }

    public void Pause()
    {
        if (!toggle)
        {
            AudioListener.volume = 0f;
            toggle = !toggle;
        }
        PauseMenuObj.SetActive(true);
        IsPaused = true;

        // Freeze time
        Time.timeScale = 0f;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToggleHowToPlayPanel()
    {
        if (HowToPlayPanel.activeInHierarchy)
        {
            HowToPlayPanel.SetActive(false);
        }
        else
        {
            HowToPlayPanel.SetActive(true);
        }

    }
}