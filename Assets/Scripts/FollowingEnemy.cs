using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Smart patrol/chase enemy for the 2.5D (XY-plane) game.
//
// Behaviour:
//   Patrol      - moves between waypoints, pausing briefly at each.
//   Chase       - when the player enters the vision cone (range + angle + line of sight),
//                 the enemy chases the player.
//   ReturnDelay - when the player leaves sight, the enemy stops and waits
//                 'returnToPatrolDelay' seconds, then resumes patrol
//                 (re-acquiring the player immediately if it reappears).
//
// All movement happens on the XY plane (z is locked to the spawn position),
// so it works in any level without a baked NavMesh.
public class FollowingEnemy : MonoBehaviour, IEnemyObservable
{
    enum State { Patrol, Chase, ReturnDelay }

    [Header("Patrol")]
    [Tooltip("Points the enemy patrols between. Leave empty for a stationary sentry.")]
    [SerializeField] List<Transform> waypoints = new List<Transform>();
    [Tooltip("How close (in world units) counts as 'reached' a waypoint.")]
    [SerializeField] float waypointRadius = 0.5f;
    [Tooltip("Seconds to pause at each waypoint before moving to the next.")]
    [SerializeField] float waypointPause = 1f;
    [Tooltip("Bounce back and forth along the waypoints instead of looping.")]
    [SerializeField] bool pingPong = false;

    [Header("Vision")]
    [Tooltip("How far the enemy can see.")]
    [FormerlySerializedAs("detectionRange")]
    [SerializeField] float visionRange = 10f;
    [Tooltip("Full width of the vision cone, in degrees.")]
    [FormerlySerializedAs("fovAngle")]
    [SerializeField] float visionAngle = 90f;
    [Tooltip("Layers whose SOLID colliders block line of sight (walls). Trigger colliders (e.g. pickups) never block. Set to Nothing to let the enemy see through walls.")]
    [SerializeField] LayerMask obstacleMask = ~0; // Everything by default

    [Header("Movement")]
    [SerializeField] float patrolSpeed = 2f;
    [SerializeField] float chaseSpeed = 5f;
    [Tooltip("How fast the vision direction turns toward its target, in degrees/second.")]
    [SerializeField] float turnSpeed = 360f;

    [Header("Wall Avoidance")]
    [Tooltip("How far ahead (world units) to look for walls while moving. Larger = reacts earlier.")]
    [SerializeField] float avoidLookAhead = 4f;
    [Tooltip("Radius of the 'feeler' used to detect walls. Roughly the body's half-width; keeps the enemy this far off walls.")]
    [SerializeField] float avoidRadius = 1.5f;

    [Header("Behaviour")]
    [Tooltip("Seconds the enemy waits (stopped) after losing sight before returning to patrol.")]
    [SerializeField] float returnToPatrolDelay = 3f;
    [Tooltip("If the player gets this close, it is caught (triggers the crash sequence).")]
    [SerializeField] float attackRange = 1.5f;
    [Tooltip("While patrolling, if the enemy can't get any closer to its target waypoint for this many seconds (e.g. wall-avoidance has boxed it into a dead end), it gives up and moves on to the next waypoint.")]
    [SerializeField] float patrolStuckTimeout = 1.5f;

    [Header("Feedback (optional)")]
    [SerializeField] Light visionLight;
    [SerializeField] Color patrolColor = new Color(0.2f, 0.9f, 1f); // cyan (stands out from the level's orange blocks)
    [SerializeField] Color chaseColor = Color.red;
    [SerializeField] AudioClip detectionSound;

    State state = State.Patrol;
    AudioSource audioSource;
    Transform player;
    CollisionHandler playerCollisionHandler;

    Vector3 facingDir = Vector3.right; // logical vision direction, in the XY plane
    int waypointIndex = 0;
    int waypointStep = 1;              // direction of travel for ping-pong
    float waypointWaitTimer = 0f;
    float lostSightTimer = 0f;
    float patrolStuckTimer = 0f;       // time spent unable to approach the current waypoint
    float bestWaypointDistance = Mathf.Infinity; // closest we've gotten to the current waypoint
    float planeZ;                      // z is locked to the spawn position
    float lightForwardOffset;          // distance from center to the enemy's front (for the vision light)
    Vector3 homePosition;              // guard post (used by stationary, waypoint-less sentries)
    Vector3 homeFacing = Vector3.right;
    bool hasCaughtPlayer = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        planeZ = transform.position.z;

        // Place the vision light just in front of the (possibly large) body so the cone is visible.
        var rend = GetComponent<Renderer>();
        lightForwardOffset = rend != null ? rend.bounds.extents.x : 0f;

        var playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO.transform;
            playerCollisionHandler = playerGO.GetComponent<CollisionHandler>();
        }
        else
        {
            Debug.LogWarning("FollowingEnemy: no GameObject tagged 'Player' found.", this);
        }

        if (waypoints != null && waypoints.Count > 0 && waypoints[0] != null)
            facingDir = DirInPlane(waypoints[0].position - transform.position, facingDir);
        else
            facingDir = DirInPlane(transform.right, facingDir); // stationary sentry guards along its local +X (rotate the enemy to aim it)

        // Remember the guard post so a stationary sentry can return to it after a chase.
        homePosition = transform.position;
        homeFacing = facingDir;

        ApplyVisionLight(patrolColor);
    }

    void Update()
    {
        if (player != null)
        {
            switch (state)
            {
                case State.Patrol: UpdatePatrol(); break;
                case State.Chase: UpdateChase(); break;
                case State.ReturnDelay: UpdateReturnDelay(); break;
            }

            TryCatchPlayer();
        }

        // Observer Pattern: let obstacles react to this enemy's patrol each frame.
        NotifyObservers();
    }

    // ----------------------------------------------------- Observer (subject)

    private readonly List<IEnemyObserver> observers = new List<IEnemyObserver>();

    public void AddObserver(IEnemyObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IEnemyObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            if (observer != null)
            {
                observer.OnEnemyMoved(this);
            }
            else
            {
                Debug.LogWarning("Null observer found in FollowingEnemy observers list!");
            }
        }
    }

    // ---------------------------------------------------------------- States

    void UpdatePatrol()
    {
        if (CanSeePlayer()) { EnterChase(); return; }

        if (!HasWaypoints()) { ReturnToPost(); return; } // stationary sentry

        Vector3 target = waypoints[waypointIndex].position;
        float dist = DistanceInPlane(target);

        if (dist <= waypointRadius)
        {
            waypointWaitTimer += Time.deltaTime;
            if (waypointWaitTimer >= waypointPause)
            {
                waypointWaitTimer = 0f;
                AdvanceWaypoint();
            }
            return;
        }

        // If wall-avoidance has boxed us into a dead end and we can't get any
        // closer to this waypoint for a while, skip it so patrol never locks up.
        if (dist < bestWaypointDistance - 0.1f)
        {
            bestWaypointDistance = dist;
            patrolStuckTimer = 0f;
        }
        else
        {
            patrolStuckTimer += Time.deltaTime;
            if (patrolStuckTimer >= patrolStuckTimeout)
            {
                AdvanceWaypoint();
                return;
            }
        }

        MoveTowards(target, patrolSpeed);
    }

    void EnterChase()
    {
        state = State.Chase;
        ApplyVisionLight(chaseColor);
        if (detectionSound != null && audioSource != null && !audioSource.isPlaying)
            audioSource.PlayOneShot(detectionSound);
    }

    void UpdateChase()
    {
        if (!CanSeePlayer())
        {
            state = State.ReturnDelay;
            lostSightTimer = 0f;
            return;
        }

        MoveTowards(player.position, chaseSpeed);
    }

    void UpdateReturnDelay()
    {
        // Player reappeared -> resume the chase immediately.
        if (CanSeePlayer()) { EnterChase(); return; }

        // Otherwise hold position and count down before returning to patrol.
        lostSightTimer += Time.deltaTime;
        if (lostSightTimer >= returnToPatrolDelay)
        {
            state = State.Patrol;
            waypointWaitTimer = 0f;
            bestWaypointDistance = Mathf.Infinity; // we likely moved far during the chase
            patrolStuckTimer = 0f;
            ApplyVisionLight(patrolColor);
        }
    }

    // Stationary sentry: walk back to the guard post, then hold and face the original direction.
    void ReturnToPost()
    {
        if (DistanceInPlane(homePosition) > waypointRadius)
        {
            MoveTowards(homePosition, patrolSpeed);
        }
        else
        {
            transform.position = new Vector3(homePosition.x, homePosition.y, planeZ);
            facingDir = Vector3.RotateTowards(facingDir, homeFacing, Mathf.Deg2Rad * turnSpeed * Time.deltaTime, 0f);
            AimVisionLight();
        }
    }

    // ------------------------------------------------------------- Movement

    void MoveTowards(Vector3 worldTarget, float speed)
    {
        Vector3 target = new Vector3(worldTarget.x, worldTarget.y, planeZ);

        Vector3 toTarget = DirInPlane(target - transform.position, facingDir);
        Vector3 moveDir = AvoidWalls(toTarget);

        // Face the actual direction of travel; if boxed in, keep aiming at the target.
        Vector3 faceDir = moveDir.sqrMagnitude > 0.0001f ? moveDir : toTarget;
        facingDir = Vector3.RotateTowards(facingDir, faceDir, Mathf.Deg2Rad * turnSpeed * Time.deltaTime, 0f);

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            float step = Mathf.Min(speed * Time.deltaTime, DistanceInPlane(target));
            Vector3 pos = transform.position + moveDir * step;
            transform.position = new Vector3(pos.x, pos.y, planeZ); // keep z locked to the spawn plane
        }

        AimVisionLight();
    }

    // Steer around solid walls on the XY plane. Returns a clear direction close to
    // 'desiredDir', or Vector3.zero when every probed direction is blocked (boxed in).
    Vector3 AvoidWalls(Vector3 desiredDir)
    {
        if (obstacleMask.value == 0) return desiredDir;   // configured to move through walls
        if (PathClear(desiredDir)) return desiredDir;

        // Try progressively wider angles to either side and take the first clear one.
        float[] probeAngles = { 25f, -25f, 50f, -50f, 75f, -75f, 100f, -100f, 130f, -130f };
        foreach (float a in probeAngles)
        {
            Vector3 candidate = Quaternion.Euler(0f, 0f, a) * desiredDir; // rotate within the XY plane
            if (PathClear(candidate)) return candidate;
        }
        return Vector3.zero; // surrounded — hold position this frame
    }

    // True if a feeler swept along 'dir' hits no solid wall (ignores self, the player, and triggers).
    bool PathClear(Vector3 dir)
    {
        var hits = Physics.SphereCastAll(transform.position, avoidRadius, dir, avoidLookAhead,
                                         obstacleMask, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            Transform ht = h.collider.transform;
            if (ht == transform || ht.IsChildOf(transform)) continue;          // ignore self
            if (player != null && (ht == player || ht.IsChildOf(player))) continue; // the player isn't a wall
            return false;
        }
        return true;
    }

    void AdvanceWaypoint()
    {
        bestWaypointDistance = Mathf.Infinity; // fresh progress tracking for the new target
        patrolStuckTimer = 0f;

        if (pingPong)
        {
            int next = waypointIndex + waypointStep;
            if (next < 0 || next >= waypoints.Count)
            {
                waypointStep = -waypointStep;
                next = waypointIndex + waypointStep;
            }
            waypointIndex = Mathf.Clamp(next, 0, waypoints.Count - 1);
        }
        else
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Count;
        }
    }

    // --------------------------------------------------------------- Vision

    bool CanSeePlayer()
    {
        Vector3 toPlayer = DirInPlane(player.position - transform.position, facingDir);
        float dist = DistanceInPlane(player.position);

        if (dist > visionRange) return false;
        if (Vector3.Angle(facingDir, toPlayer) > visionAngle * 0.5f) return false;
        if (!HasLineOfSight()) return false;

        return true;
    }

    // True if no solid wall sits between the enemy and the player.
    // Ignores the enemy's own colliders, the player, and trigger colliders (pickups).
    bool HasLineOfSight()
    {
        if (obstacleMask.value == 0) return true; // configured to see through walls

        Vector3 origin = transform.position;
        Vector3 toPlayer = player.position - origin;
        float dist = toPlayer.magnitude;
        if (dist < 0.001f) return true;

        var hits = Physics.RaycastAll(origin, toPlayer.normalized, dist, obstacleMask, QueryTriggerInteraction.Ignore);
        foreach (var h in hits)
        {
            Transform ht = h.collider.transform;
            if (ht == transform || ht.IsChildOf(transform)) continue; // ignore self
            if (ht == player || ht.IsChildOf(player)) continue;       // the player isn't a wall
            return false;                                             // a solid wall blocks the view
        }
        return true;
    }

    void TryCatchPlayer()
    {
        if (hasCaughtPlayer || playerCollisionHandler == null) return;
        if (DistanceInPlane(player.position) <= attackRange)
        {
            hasCaughtPlayer = true;
            playerCollisionHandler.StartCrashSequence();
        }
    }

    // ---------------------------------------------------------------- Helpers

    bool HasWaypoints()
    {
        return waypoints != null && waypoints.Count > 0 && waypoints[waypointIndex] != null;
    }

    // Distance between this enemy and a world point, ignoring the z (depth) axis.
    float DistanceInPlane(Vector3 world)
    {
        Vector2 a = new Vector2(transform.position.x, transform.position.y);
        Vector2 b = new Vector2(world.x, world.y);
        return Vector2.Distance(a, b);
    }

    // Flatten a vector onto the XY plane and normalise it (keeps 'fallback' if zero-length).
    Vector3 DirInPlane(Vector3 v, Vector3 fallback)
    {
        v.z = 0f;
        return v.sqrMagnitude > 0.0001f ? v.normalized : fallback;
    }

    void ApplyVisionLight(Color color)
    {
        if (visionLight == null) return;
        visionLight.type = LightType.Spot;
        visionLight.spotAngle = visionAngle;
        // Reach past the (possibly large) body so the cone lands on walls beyond it.
        visionLight.range = visionRange + lightForwardOffset;
        visionLight.color = color;
        AimVisionLight();
    }

    void AimVisionLight()
    {
        if (visionLight == null) return;
        // Keep the light at the body centre and just point it along the facing direction;
        // its extended range (set in ApplyVisionLight) projects the cone past the body.
        visionLight.transform.position = transform.position;
        visionLight.transform.rotation = Quaternion.LookRotation(facingDir, Vector3.forward);
    }

    void OnDrawGizmosSelected()
    {
        // In play mode show the real facing; in the editor approximate it with +X.
        Vector3 f = (Application.isPlaying && facingDir.sqrMagnitude > 0.0001f) ? facingDir : Vector3.right;

        Gizmos.color = Color.yellow;
        Vector3 left = Quaternion.Euler(0f, 0f, visionAngle * 0.5f) * f;
        Vector3 right = Quaternion.Euler(0f, 0f, -visionAngle * 0.5f) * f;
        Gizmos.DrawRay(transform.position, left * visionRange);
        Gizmos.DrawRay(transform.position, right * visionRange);
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
