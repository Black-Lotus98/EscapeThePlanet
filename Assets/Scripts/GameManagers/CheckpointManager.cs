using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private readonly List<ICheckpointable> checkpointables = new List<ICheckpointable>();
    private readonly List<IRespawnResettable> resettables = new List<IRespawnResettable>();

    private readonly Dictionary<ICheckpointable, object> snapshot = new Dictionary<ICheckpointable, object>();

    private readonly HashSet<object> activatedSources = new HashSet<object>();

    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    private Quaternion baselineRotation = Quaternion.identity;
    private bool hasSnapshot = false;

    private int currentAttempts;

    private GameObject player;
    private Rigidbody playerBody;
    private PlayerIntegrity playerIntegrity;
    private HeartsManager heartsManager;
    private GameOverController gameOverController;

    private int MaxAttempts { get { return DifficultyManager.MaxAttemptsFor(DifficultyManager.Current); } }

    public bool IsUnlimited { get { return MaxAttempts == DifficultyManager.Unlimited; } }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        currentAttempts = MaxAttempts;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerBody = player.GetComponent<Rigidbody>();
            playerIntegrity = player.GetComponent<PlayerIntegrity>();
        }
        heartsManager = FindObjectOfType<HeartsManager>();
        gameOverController = FindObjectOfType<GameOverController>();

        StartCoroutine(InitializeBaseline());
    }

    // Wait one frame so every ICheckpointable's Start/Awake has run and nothing has been
    // collected yet, then discover them all and capture checkpoint #0 (the start point).
    private IEnumerator InitializeBaseline()
    {
        yield return null;

        DiscoverParticipants();

        if (player != null)
        {
            respawnPosition = player.transform.position;
            respawnRotation = player.transform.rotation;
            baselineRotation = player.transform.rotation; 
        }

        CaptureSnapshot();
        RefreshHearts();
    }

    private void DiscoverParticipants()
    {
        checkpointables.Clear();
        resettables.Clear();

       
        MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour is ICheckpointable checkpointable)
            {
                checkpointables.Add(checkpointable);
            }
            if (behaviour is IRespawnResettable resettable)
            {
                resettables.Add(resettable);
            }
        }
    }

    
    public void ActivateCheckpoint(Transform checkpointTransform, object source)
    {
        if (source != null && activatedSources.Contains(source))
        {
            return;
        }
        if (source != null)
        {
            activatedSources.Add(source);
        }

        if (source is GameObject sourceObject)
        {
            CheckpointVisual visual = sourceObject.GetComponent<CheckpointVisual>();
            if (visual != null)
            {
                visual.Activate();
            }
        }

        respawnRotation = baselineRotation;
        respawnPosition = ComputeRespawnOnTop(checkpointTransform);

        if (playerIntegrity == null && player != null)
        {
            playerIntegrity = player.GetComponent<PlayerIntegrity>();
        }
        if (playerIntegrity != null)
        {
            playerIntegrity.Refill();
        }

        CaptureSnapshot();

        currentAttempts = MaxAttempts;
        RefreshHearts();
    }

    private Vector3 ComputeRespawnOnTop(Transform checkpointTransform)
    {
        if (checkpointTransform == null)
        {
            return player != null ? player.transform.position : respawnPosition;
        }

        Collider padCollider = null;
        foreach (Collider c in checkpointTransform.GetComponentsInChildren<Collider>())
        {
            if (!c.isTrigger) { padCollider = c; break; }
        }
        if (padCollider == null)
        {
            return checkpointTransform.position + Vector3.up * 2f;
        }

        float clearance = 0.5f;
        if (player != null)
        {
            Collider playerCollider = player.GetComponent<Collider>();
            if (playerCollider == null) playerCollider = player.GetComponentInChildren<Collider>();
            if (playerCollider != null) clearance = playerCollider.bounds.extents.y + 0.15f;
        }

        Bounds b = padCollider.bounds;
        return new Vector3(b.center.x, b.max.y + clearance, b.center.z);
    }

    private void CaptureSnapshot()
    {
        snapshot.Clear();
        foreach (ICheckpointable checkpointable in checkpointables)
        {
            if (checkpointable != null)
            {
                snapshot[checkpointable] = checkpointable.CaptureState();
            }
        }
        hasSnapshot = true;
    }

    public bool OnPlayerDied()
    {
        if (!IsUnlimited)
        {
            currentAttempts--;
        }
        RefreshHearts();

        if (!IsUnlimited && currentAttempts <= 0)
        {
            TriggerGameOver();
            return false;
        }

        Respawn();
        return true;
    }

    private void Respawn()
    {
        if (hasSnapshot)
        {
            foreach (KeyValuePair<ICheckpointable, object> entry in snapshot)
            {
                if (entry.Key != null)
                {
                    entry.Key.RestoreState(entry.Value);
                }
            }
        }

        foreach (IRespawnResettable resettable in resettables)
        {
            if (resettable != null)
            {
                resettable.ResetToSpawn();
            }
        }

        ClearBullets();

        if (player != null)
        {
            player.transform.SetPositionAndRotation(respawnPosition, respawnRotation);
            if (playerBody != null)
            {
                playerBody.linearVelocity = Vector3.zero;
                playerBody.angularVelocity = Vector3.zero;
            }
        }
    }

    private void ClearBullets()
    {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
    }

    private void RefreshHearts()
    {
        if (heartsManager == null)
        {
            heartsManager = FindObjectOfType<HeartsManager>();
        }
        if (heartsManager != null)
        {
            if (IsUnlimited)
            {
                heartsManager.SetHearts(-1, -1);
            }
            else
            {
                heartsManager.SetHearts(currentAttempts, MaxAttempts);
            }
        }
    }

    private void TriggerGameOver()
    {
        if (gameOverController == null)
        {
            gameOverController = FindObjectOfType<GameOverController>();
        }
        if (gameOverController != null)
        {
            gameOverController.ShowGameOver();
        }
        else
        {
            Debug.LogWarning("CheckpointManager: no GameOverController found - restarting level.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}
