using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this script must be attached to the game master object and then to be referenced in the Performance panel 
public class LoadStatus : MonoBehaviour
{
    [SerializeField] Text NumberOfDeathsText;
    [SerializeField] Text LastLevelReachedText;

    public void LoadProgress()
    {
        PlayerData data = SaveData.LoadPlayer();

        if (data != null)
        {
            NumberOfDeathsText.text = data.NumberOfDeaths.ToString();
            LastLevelReachedText.text = "Level " + (data.LastLevelReached).ToString();
        }
        else
        {
            Debug.LogError("Failed to load player data.");
        }
    }
}
