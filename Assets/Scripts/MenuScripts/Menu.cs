using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    public void RestartGame()
    {
        SaveData.ResetPlayerData(null);
        SaveManager.ResetGameData(null);
        GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameManager>().RestartGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
