using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Memento Pattern - Caretaker. A SCENE-SCOPED component (place on the GameMaster
// object). Because it is not DontDestroyOnLoad, re-entering a level creates a fresh
// manager, so checkpoint progress never survives leaving the level (matches the design:
// in-scene only, resets on exit-to-menu).
//
// Responsibilities:
//  - Discover every ICheckpointable / IRespawnResettable in the scene at the first
//    frame (all are still active then) and cache references (so they can be restored
//    even after being deactivated when collected).
//  - Capture a snapshot when a checkpoint is first activated (capture-once per source).
//  - On death, decrement attempts and either respawn (restore snapshot + reset hazards
//    + reposition the player) or trigger Game-Over when revives run out.
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    private readonly List<ICheckpointable> checkpointables = new List<ICheckpointable>();
    private readonly List<IRespawnResettable> resettables = new List<IRespawnResettable>();

    private readonly Dictionary<ICheckpointable, object> snapshot = new Dictionary<ICheckpointable, object>();

    // Capture-once guard: checkpoint sources (pad GameObjects) that have already fired.
    private readonly HashSet<object> activatedSources = new HashSet<object>();

    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    private Quaternion baselineRotation = Quaternion.identity; // launch orientation, reused for safe upright respawns
    private bool hasSnapshot = false;

    private int maxAttempts;
    private int currentAttempts;

    private GameObject player;
    private Rigidbody playerBody;
    private HeartsManager heartsManager;
    private GameOverController gameOverController;

    public bool IsUnlimited { get { return maxAttempts == DifficultyManager.Unlimited; } }

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
        Difficulty difficulty = DifficultyManager.Current;
        maxAttempts = DifficultyManager.MaxAttemptsFor(difficulty);
        currentAttempts = maxAttempts;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerBody = player.GetComponent<Rigidbody>();
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
            baselineRotation = player.transform.rotation; // remember the upright launch orientation
        }

        CaptureSnapshot();
        RefreshHearts();
    }

    private void DiscoverParticipants()
    {
        checkpointables.Clear();
        resettables.Clear();

        // Active objects only (default). At baseline nothing is collected, so every
        // collectible is still active and gets cached; later deactivation keeps the ref.
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

    // Called by CollisionHandler (CheckpointState pad) and CustomTeleporter (TP pad).
    // 'source' identifies the checkpoint so it only captures once; re-touching does nothing.
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

        // Visual feedback (e.g. a teleport pad turning blue) on first activation.
        if (source is GameObject sourceObject)
        {
            CheckpointVisual visual = sourceObject.GetComponent<CheckpointVisual>();
            if (visual != null)
            {
                visual.Activate();
            }
        }

        // Respawn ON TOP of the pad's solid collider (centered, just clearing the surface)
        // with the upright launch orientation. Using the pad's pivot/rotation spawned the
        // player inside/below the tilted pad -> instant crash.
        respawnRotation = baselineRotation;
        respawnPosition = ComputeRespawnOnTop(checkpointTransform);

        CaptureSnapshot();

        // Reaching a new checkpoint refills revive attempts.
        currentAttempts = maxAttempts;
        RefreshHearts();
    }

    // Returns a point centered on top of the checkpoint pad's solid collider, raised so the
    // player's own collider clears the surface (so respawn never lands inside/below the pad).
    private Vector3 ComputeRespawnOnTop(Transform checkpointTransform)
    {
        if (checkpointTransform == null)
        {
            return player != null ? player.transform.position : respawnPosition;
        }

        // The pad's solid (non-trigger) collider.
        Collider padCollider = null;
        foreach (Collider c in checkpointTransform.GetComponentsInChildren<Collider>())
        {
            if (!c.isTrigger) { padCollider = c; break; }
        }
        if (padCollider == null)
        {
            return checkpointTransform.position + Vector3.up * 2f;
        }

        // Clearance = the player's own collider half-height, so it rests just above the pad.
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

    // Called from CollisionHandler when the player dies (non-tutorial scenes).
    // Returns true if the player respawned, false if it was a Game-Over.
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
                heartsManager.SetHearts(currentAttempts, maxAttempts);
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
            // Graceful fallback until a Game-Over panel is authored: restart the level
            // from its true start (a fresh scene load resets attempts and checkpoint).
            Debug.LogWarning("CheckpointManager: no GameOverController found - restarting level.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}
