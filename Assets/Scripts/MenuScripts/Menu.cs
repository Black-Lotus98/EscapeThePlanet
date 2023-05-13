using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
    PlayerData data;

    public void RestartGame()
    {
        SaveData.ResetPlayerData(data);
        GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameManager>().RestartGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        SaveData.LoadPlayer();
    }

}
