using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadStatus : MonoBehaviour
{
    [SerializeField] Text NumberOfDeathsText;
    [SerializeField] Text LastLevelReachedText;
    [SerializeField] GameObject PerformancePanel;


    void Update()
    {
        if (PerformancePanel.activeInHierarchy)
        {
            LoadProgress();
        }
    }
    public void LoadProgress()
    {
        PlayerData data = SaveData.LoadPlayer();

        NumberOfDeathsText.text = data.NumberOfDeaths.ToString();
        LastLevelReachedText.text = "Level " + (data.LastLevelReached).ToString();
    }
}
