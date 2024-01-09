using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsUIObserver : MonoBehaviour, IUIObserver<StarsManager>
{

    private StarsManager starsManager;
    [SerializeField] Image[] StarImages;
    [SerializeField] Sprite CollectedStar;
    private void Awake()
    {
        starsManager = GameObject.FindObjectOfType<StarsManager>();
        if (starsManager != null)
        {
            // this will subscribe the observer to the player
            starsManager.AddObserver(this);
            starsManager.NotifyObservers(UIState.StarsState);
        }
        else
        {
            Debug.LogError("Player not found.");
        }
    }

    public void OnStateChange(StarsManager aStarsManager, UIState state)
    {
        if (state == UIState.StarsState)
        {
            // Debug.Log($"I am notified of {this} and I am Stars UI Observer.");
            UpdateStarsUI(aStarsManager);
        }
    }
    private void UpdateStarsUI(StarsManager aStarsManager)
    {
        //The -1 here because the array index start from 0 while the start Id start from 1
        int starId = aStarsManager.CollectedStarsCounter-1;
        Debug.Log($"starId: {starId}");
        StarImages[starId].GetComponent<Image>().sprite = CollectedStar;
    }
}