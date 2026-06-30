using UnityEngine;
using UnityEngine.UI;

// Hearts HUD widget. Mirrors StarsUIObserver. Repaints a row of heart Images from the
// HeartsManager. On Easy (unlimited) the whole HUD is hidden.
public class HeartsUIObserver : MonoBehaviour, IUIObserver<HeartsManager>
{
    [Header("UI References")]
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [Tooltip("Optional container to hide on Easy (unlimited). Defaults to this GameObject.")]
    [SerializeField] private GameObject heartsContainer;

    private HeartsManager heartsManager;
    private bool isRegistered = false;

    private void Awake()
    {
        if (heartImages == null || heartImages.Length == 0)
        {
            Debug.LogError("HeartImages array is not assigned or empty in HeartsUIObserver!");
            enabled = false;
            return;
        }

        if (fullHeart == null)
        {
            Debug.LogError("FullHeart sprite is not assigned in HeartsUIObserver!");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        RegisterWithManager();
    }

    private void RegisterWithManager()
    {
        if (isRegistered) return;

        heartsManager = FindObjectOfType<HeartsManager>();
        if (heartsManager == null)
        {
            Debug.LogError("HeartsManager not found in scene!");
            enabled = false;
            return;
        }

        heartsManager.AddObserver(this);
        heartsManager.NotifyObservers(UIState.HeartsChanged);
        isRegistered = true;
    }

    public void OnStateChange(HeartsManager manager, UIState state)
    {
        if (state == UIState.HeartsChanged)
        {
            UpdateHeartsUI(manager);
        }
    }

    private void UpdateHeartsUI(HeartsManager manager)
    {
        if (heartImages == null || manager == null)
        {
            return;
        }

        GameObject container = heartsContainer != null ? heartsContainer : gameObject;

        // Unlimited (Easy): hide the whole HUD.
        if (manager.IsUnlimited)
        {
            container.SetActive(false);
            return;
        }

        container.SetActive(true);

        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;

            // Hide hearts beyond the current max (e.g. Hard shows 3, Medium shows 5).
            if (i >= manager.MaxHearts)
            {
                heartImages[i].enabled = false;
                continue;
            }

            if (i < manager.CurrentHearts)
            {
                heartImages[i].enabled = true;
                heartImages[i].sprite = fullHeart;
            }
            else if (emptyHeart != null)
            {
                heartImages[i].enabled = true;
                heartImages[i].sprite = emptyHeart;
            }
            else
            {
                heartImages[i].enabled = false;
            }
        }
    }
}
