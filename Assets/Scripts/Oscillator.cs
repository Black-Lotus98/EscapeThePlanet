using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Oscillates an object back and forth along 'movementsVector' using a sine wave.
//
// Smart freeze: when a FollowingEnemy comes near the obstacle's travel path, the
// obstacle eases to its "open" spot (start + openOffset) and holds there until the
// enemy has passed, then resumes its swing seamlessly. This stops it shoving or
// blocking the patrolling enemy, while it still moves normally for the player.
//
// Proximity is delivered via the Observer Pattern: this obstacle subscribes to the
// scene's FollowingEnemy subjects (IEnemyObservable) and reacts to their movement
// in OnEnemyMoved, instead of polling the scene every frame.
//
//'openOffset' is authored like 'movementsVector' - a relative offset from the start
// position - and defaults to it (see Reset), so a new obstacle pulls aside along the
// same axis it travels; flip/adjust it per obstacle to point at the clear side.
public class Oscillator : MonoBehaviour, IEnemyObserver
{
    Vector3 StartingPosition;
    [SerializeField] Vector3 movementsVector;
    [SerializeField] [Range(-1,1)] float movementsFactor;
    [SerializeField] float period = 5f;
    [SerializeField] bool fullcycles= false;

    [Header("Smart Freeze (yields to a patrolling FollowingEnemy)")]
    [Tooltip("If a FollowingEnemy comes within this distance of the obstacle's travel path, it pulls aside to its open spot until the enemy passes. Set 0 to disable.")]
    [SerializeField] float enemyFreezeRadius = 7f;
    [Tooltip("Seconds to ease into / out of the parked (open) state.")]
    [SerializeField] float freezeBlendTime = 0.3f;
    [Tooltip("Offset from the start position the obstacle pulls aside to while a patrol enemy is near - the 'open' spot that clears the patrol route. Authored like Movements Vector and defaults to it; flip/adjust so it points at the clear side.")]
    [SerializeField] Vector3 openOffset;

    const float tau = Mathf.PI * 2;

    float phaseTime = 0f;      // oscillation clock; only advances while NOT frozen, so motion resumes seamlessly
    float blend = 1f;          // 0 = parked at open spot, 1 = full oscillation

    // Observer Pattern (observer side): enemies we subscribed to, and the ones currently near our path.
    readonly List<FollowingEnemy> subscribedEnemies = new List<FollowingEnemy>();
    readonly HashSet<FollowingEnemy> nearbyEnemies = new HashSet<FollowingEnemy>();

    void Reset()
    {
        // Default the open offset to the travel vector so a freshly added obstacle
        // pulls aside along the same axis it oscillates on.
        openOffset = movementsVector;
    }

    void Start()
    {
        StartingPosition = transform.position;

        // Subscribe to every FollowingEnemy so they push their movement to us.
        foreach (var enemy in FindObjectsByType<FollowingEnemy>(FindObjectsSortMode.None))
        {
            enemy.AddObserver(this);
            subscribedEnemies.Add(enemy);
        }
    }

    void OnDestroy()
    {
        foreach (var enemy in subscribedEnemies)
        {
            if (enemy != null) enemy.RemoveObserver(this);
        }
        subscribedEnemies.Clear();
        nearbyEnemies.Clear();
    }

    // Observer callback: a subscribed enemy moved - track whether it sits on our travel path.
    public void OnEnemyMoved(FollowingEnemy enemy)
    {
        if (enemy == null || enemyFreezeRadius <= 0f) return;

        float minFactor = fullcycles ? -1f : 0f;
        Vector3 a = StartingPosition + movementsVector * minFactor;
        Vector3 b = StartingPosition + movementsVector;
        float r2 = enemyFreezeRadius * enemyFreezeRadius;

        if (SqrDistanceToSegment(enemy.transform.position, a, b) <= r2)
        {
            nearbyEnemies.Add(enemy);
        }
        else
        {
            nearbyEnemies.Remove(enemy);
        }
    }

    void Update()
    {
        if(period <= Mathf.Epsilon)
        {
            return;
        }

        nearbyEnemies.RemoveWhere(e => e == null); // drop any destroyed enemies
        bool freeze = nearbyEnemies.Count > 0;

        // Only advance the oscillation clock while active, so it resumes seamlessly.
        if (!freeze)
        {
            phaseTime += Time.deltaTime;
        }

        // Smoothly blend between parked (0) and oscillating (1).
        float blendTarget = freeze ? 0f : 1f;
        float blendRate = freezeBlendTime > Mathf.Epsilon ? Time.deltaTime / freezeBlendTime : 1f;
        blend = Mathf.MoveTowards(blend, blendTarget, blendRate);

        movementsFactor = OscFactor(phaseTime);
        Vector3 oscillatingPos = StartingPosition + movementsVector * movementsFactor;
        Vector3 openPos = StartingPosition + openOffset;
        transform.position = Vector3.Lerp(openPos, oscillatingPos, blend);
    }

    float OscFactor(float t)
    {
        float cycles = t / period;
        float rawSinWave = Mathf.Sin(cycles * tau);
        return fullcycles ? rawSinWave : (rawSinWave + 1f) / 2f;
    }

    // Squared distance from point p to the segment [a, b].
    static float SqrDistanceToSegment(Vector3 p, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        float len2 = ab.sqrMagnitude;
        if (len2 < 1e-6f) return (p - a).sqrMagnitude; // degenerate: a == b
        float t = Mathf.Clamp01(Vector3.Dot(p - a, ab) / len2);
        Vector3 projection = a + ab * t;
        return (p - projection).sqrMagnitude;
    }

    void OnDrawGizmosSelected()
    {
        // In edit mode the live start hasn't been captured yet, so use the transform.
        Vector3 start = Application.isPlaying ? StartingPosition : transform.position;
        float minFactor = fullcycles ? -1f : 0f;
        Vector3 endA = start + movementsVector * minFactor;
        Vector3 endB = start + movementsVector;

        // Swing path + its two ends.
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(endA, endB);
        Gizmos.DrawWireSphere(endA, 0.4f);
        Gizmos.DrawWireSphere(endB, 0.4f);

        // Open spot it pulls aside to for a patrol enemy.
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(start + openOffset, 0.7f);

        // Detection radius around the travel path.
        if (enemyFreezeRadius > 0f)
        {
            Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.35f);
            Gizmos.DrawWireSphere((endA + endB) * 0.5f, enemyFreezeRadius);
        }
    }
}
