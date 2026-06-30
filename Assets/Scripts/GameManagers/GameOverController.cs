using UnityEngine;
using UnityEngine.SceneManagement;

// Shows the Game-Over panel when the player runs out of revives at a checkpoint.
// Retry restarts the whole level from its true start (a fresh scene load makes a fresh
// CheckpointManager, so attempts reset and the checkpoint is cleared). Exit returns to
// the main menu. Wire Retry/ExitToMenu to the panel's buttons in the editor.
public class GameOverController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
