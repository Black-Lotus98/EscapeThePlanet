using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsUIObserver : MonoBehaviour, IUIObserver<StarsManager>
{
    [Header("UI References")]
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite collectedStar;
    
    private StarsManager starsManager;
    private bool isRegistered = false;

    private void Awake()
    {
        // Validate UI components
        if (starImages == null || starImages.Length == 0)
        {
            Debug.LogError("StarImages array is not assigned or empty in StarsUIObserver!");
            enabled = false;
            return;
        }

        if (collectedStar == null)
        {
            Debug.LogError("CollectedStar sprite is not assigned in StarsUIObserver!");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        // Register with manager in Start() to ensure proper initialization order
        RegisterWithManager();
    }

    private void RegisterWithManager()
    {
        if (isRegistered) return;

        // Cache component reference for better performance
        starsManager = FindObjectOfType<StarsManager>();
        
        if (starsManager == null)
        {
            Debug.LogError("StarsManager not found in scene!");
            enabled = false;
            return;
        }

        // Subscribe the observer to the stars manager
        starsManager.AddObserver(this);
        isRegistered = true;
        
        Debug.Log("StarsUIObserver registered successfully!");
    }

    public void OnStateChange(StarsManager starsManager, UIState state)
    {
        if (state == UIState.StarsState)
        {
            UpdateStarsUI(starsManager);
        }
    }

    private void UpdateStarsUI(StarsManager starsManager)
    {
        if (starImages == null || starsManager == null)
        {
            Debug.LogWarning("StarImages or StarsManager is null in UpdateStarsUI!");
            return;
        }

        try
        {
            // The -1 here because the array index starts from 0 while the star ID starts from 1
            int starId = starsManager.CollectedStarsCounter - 1;
            
            // Validate array bounds
            if (starId >= 0 && starId < starImages.Length)
            {
                if (starImages[starId] != null)
                {
                    starImages[starId].sprite = collectedStar;
                }
                else
                {
                    Debug.LogWarning($"Star image at index {starId} is null!");
                }
            }
            else
            {
                Debug.LogWarning($"Invalid star ID: {starId + 1}, array length: {starImages.Length}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating stars UI: {e.Message}");
        }
    }
}