using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    PlayerData data;
    GameData gameData;

    public void RestartGame()
    {
        SaveData.ResetPlayerData(data);
        SaveManager.ResetGameData(gameData);
        GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameManager>().RestartGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        SaveData.LoadPlayer();
    }

}
