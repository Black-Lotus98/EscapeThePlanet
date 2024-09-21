using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Settings")]
    [SerializeField] private bool isPaused = false;
    
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuObj;
    [SerializeField] private GameObject howToPlayPanel;
    
    private bool toggle = true;
    private Canvas gameCanvas;

    public bool IsPaused
    {
        get { return isPaused; }
        private set { isPaused = value; }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Unpause();
        }
        
        // Auto-assign UI references from Canvas prefab
        AutoAssignUIReferences();
    }

    private void AutoAssignUIReferences()
    {
        // Find the Canvas in the scene (should be the prefab)
        gameCanvas = FindObjectOfType<Canvas>();
        
        if (gameCanvas == null)
        {
            Debug.LogError("Canvas not found in scene!");
            return;
        }

        // Find PauseMenu within the Canvas
        pauseMenuObj = gameCanvas.transform.Find("PauseMenu")?.gameObject;
        if (pauseMenuObj == null)
        {
            Debug.LogError("PauseMenu not found in Canvas!");
        }

        // Find HowToPlayPanel within the Canvas
        howToPlayPanel = gameCanvas.transform.Find("HowToPlayPanel")?.gameObject;
        if (howToPlayPanel == null)
        {
            Debug.LogError("HowToPlayPanel not found in Canvas!");
        }

        Debug.Log("UI references auto-assigned successfully!");
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
        
        if (pauseMenuObj != null)
        {
            pauseMenuObj.SetActive(false);
        }
        
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

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
        
        if (pauseMenuObj != null)
        {
            pauseMenuObj.SetActive(true);
        }
        
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
        if (howToPlayPanel == null)
        {
            Debug.LogWarning("HowToPlayPanel is not assigned!");
            return;
        }

        howToPlayPanel.SetActive(!howToPlayPanel.activeInHierarchy);
    }
}