using UnityEngine;
using UnityEngine.UI;

public class IntegrityUIObserver : MonoBehaviour, IUIObserver<IntegrityManager>
{
    [Header("UI References")]
    [SerializeField] private Image[] pipImages;
    [SerializeField] private Sprite pipSprite;
    [SerializeField] private Color fullColor = new Color(0.35f, 0.85f, 1f, 1f);
    [SerializeField] private Color depletedColor = new Color(1f, 1f, 1f, 0.25f);
    [Tooltip("Optional container to hide when integrity is unknown. Defaults to this GameObject.")]
    [SerializeField] private GameObject pipsContainer;

    private IntegrityManager integrityManager;
    private bool isRegistered = false;

    private void Awake()
    {
        if (pipImages == null || pipImages.Length == 0)
        {
            Debug.LogWarning("PipImages array is not assigned or empty in IntegrityUIObserver!");
            enabled = false;
            return;
        }

        if (pipSprite != null)
        {
            foreach (Image pip in pipImages)
            {
                if (pip != null && pip.sprite == null)
                {
                    pip.sprite = pipSprite;
                }
            }
        }
    }

    private void Start()
    {
        RegisterWithManager();
    }

    private void RegisterWithManager()
    {
        if (isRegistered) return;

        integrityManager = FindObjectOfType<IntegrityManager>();
        if (integrityManager == null)
        {
            Debug.LogWarning("IntegrityManager not found in scene - integrity HUD disabled.");
            enabled = false;
            return;
        }

        integrityManager.AddObserver(this);
        integrityManager.NotifyObservers(UIState.IntegrityChanged);
        isRegistered = true;
    }

    public void OnStateChange(IntegrityManager manager, UIState state)
    {
        if (state == UIState.IntegrityChanged)
        {
            UpdateIntegrityUI(manager);
        }
    }

    private void UpdateIntegrityUI(IntegrityManager manager)
    {
        if (pipImages == null || manager == null)
        {
            return;
        }

        GameObject container = pipsContainer != null ? pipsContainer : gameObject;

        if (manager.MaxIntegrity < 0)
        {
            container.SetActive(false);
            return;
        }

        container.SetActive(true);

        for (int i = 0; i < pipImages.Length; i++)
        {
            if (pipImages[i] == null) continue;

            if (i >= manager.MaxIntegrity)
            {
                pipImages[i].enabled = false;
                continue;
            }

            pipImages[i].enabled = true;
            pipImages[i].color = i < manager.CurrentIntegrity ? fullColor : depletedColor;
        }
    }
}
