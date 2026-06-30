using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
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

    // New Input System (generated wrapper)
    private PlayerMovement controls;

    private void Awake()
    {
        controls = new PlayerMovement();
    }

    private void OnEnable()
    {
        controls?.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls?.Gameplay.Disable();
    }

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
        // A scene may contain more than one Canvas (e.g. the minimap canvas),
        // so pick the one that actually hosts the pause UI rather than the
        // first arbitrary Canvas returned by the engine.
        gameCanvas = null;
        Transform pauseMenuT = null;
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Canvas canvas in canvases)
        {
            // Search descendants (not just direct children) so a SafeArea wrapper
            // or other nesting under the Canvas doesn't hide the pause UI.
            Transform found = FindDeep(canvas.transform, "PauseMenu");
            if (found != null)
            {
                gameCanvas = canvas;
                pauseMenuT = found;
                break;
            }
        }

        if (gameCanvas == null)
        {
            Debug.LogError("No Canvas containing a 'PauseMenu' child was found in the scene!");
            return;
        }

        // Find PauseMenu within the Canvas
        pauseMenuObj = pauseMenuT != null ? pauseMenuT.gameObject : null;
        if (pauseMenuObj == null)
        {
            Debug.LogError("PauseMenu not found in Canvas!");
        }

        // Find HowToPlayPanel within the Canvas
        Transform howToPlayT = FindDeep(gameCanvas.transform, "HowToPlayPanel");
        howToPlayPanel = howToPlayT != null ? howToPlayT.gameObject : null;
        if (howToPlayPanel == null)
        {
            Debug.LogError("HowToPlayPanel not found in Canvas!");
        }

        // Debug.Log("UI references auto-assigned successfully!");
    }

    // Depth-first search for a descendant by name (handles nesting like a SafeArea wrapper).
    private static Transform FindDeep(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name) return child;
            Transform result = FindDeep(child, name);
            if (result != null) return result;
        }
        return null;
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
        if (controls.Gameplay.Pause.WasPressedThisFrame())
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