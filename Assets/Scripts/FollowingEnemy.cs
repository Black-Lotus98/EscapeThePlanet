using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowingEnemy : MonoBehaviour
{
    enum State { Patrol, Chase }

    [Header("Patrol")]
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float waypointRadius = 0.5f;
    [SerializeField] float scanDuration = 2f;
    [SerializeField] float scanSpeed = 60f;

    [Header("Detection")]
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float fovAngle = 90f;
    [SerializeField] LayerMask obstacleMask;

    [Header("Movement")]
    [SerializeField] float patrolSpeed = 2f;
    [SerializeField] float chaseSpeed = 5f;
    [SerializeField] float attackRange = 1.5f;

    [Header("Vision Light")]
    [SerializeField] Light visionLight;
    [SerializeField] Color patrolColor = new Color(1f, 0.5f, 0f); // orange
    [SerializeField] Color chaseColor = Color.red;

    [Header("Audio")]
    [SerializeField] AudioClip detectionSound;

    State state = State.Patrol;
    NavMeshAgent agent;
    AudioSource audioSource;
    Transform player;
    CollisionHandler playerCollisionHandler;

    int waypointIndex = 0;
    float scanTimer = 0f;
    float scanDirection = 1f;
    bool isScanning = false;
    bool hasKilled = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCollisionHandler = player.GetComponent<CollisionHandler>();

        agent.speed = patrolSpeed;

        if (visionLight != null)
        {
            visionLight.type = LightType.Spot;
            visionLight.spotAngle = fovAngle;
            visionLight.range = detectionRange;
            visionLight.color = patrolColor;
        }

        if (waypoints != null && waypoints.Count > 0)
            agent.SetDestination(waypoints[0].position);
    }

    void Update()
    {
        if (state == State.Patrol) UpdatePatrol();
        else UpdateChase();
    }

    void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            isScanning = false;
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            state = State.Chase;
            if (visionLight != null) visionLight.color = chaseColor;
            if (detectionSound != null && audioSource != null && !audioSource.isPlaying) audioSource.PlayOneShot(detectionSound);
            return;
        }

        if (waypoints == null || waypoints.Count == 0)
        {
            Scan();
            return;
        }

        bool reachedWaypoint = !agent.pathPending && agent.remainingDistance <= waypointRadius;

        if (reachedWaypoint && !isScanning)
        {
            isScanning = true;
            scanTimer = 0f;
            agent.isStopped = true;
        }

        if (isScanning) Scan();
    }

    void Scan()
    {
        scanTimer += Time.deltaTime;
        transform.Rotate(Vector3.up, scanSpeed * scanDirection * Time.deltaTime);

        if (scanTimer >= scanDuration * 0.5f)
            scanDirection = -1f;

        if (scanTimer >= scanDuration)
        {
            isScanning = false;
            scanDirection = 1f;

            if (waypoints != null && waypoints.Count > 0)
            {
                agent.isStopped = false;
                waypointIndex = (waypointIndex + 1) % waypoints.Count;
                agent.SetDestination(waypoints[waypointIndex].position);
            }
        }
    }

    void UpdateChase()
    {
        if (!CanSeePlayer())
        {
            state = State.Patrol;
            agent.speed = patrolSpeed;
            if (visionLight != null) visionLight.color = patrolColor;
            if (waypoints != null && waypoints.Count > 0)
                agent.SetDestination(waypoints[waypointIndex].position);
            return;
        }

        agent.SetDestination(player.position);

        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);

        if (!hasKilled && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            hasKilled = true;
            playerCollisionHandler.StartCrashSequence();
        }
    }

    bool CanSeePlayer()
    {
        Vector3 dirToPlayer = player.position - transform.position;
        float dist = dirToPlayer.magnitude;

        if (dist > detectionRange) return false;
        if (Vector3.Angle(transform.forward, dirToPlayer) > fovAngle * 0.5f) return false;
        if (Physics.Raycast(transform.position, dirToPlayer.normalized, dist, obstacleMask)) return false;

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -fovAngle * 0.5f, 0) * transform.forward * detectionRange);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, fovAngle * 0.5f, 0) * transform.forward * detectionRange);
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
