using UnityEngine;

public class EnemyBeam : MonoBehaviour, IRespawnResettable
{
    enum State { Idle, WindUp, Firing, Cooldown }

    [Header("Targeting")]
    [SerializeField] Transform muzzle;
    [SerializeField] float beamRange = 6f;
    [SerializeField] float beamMinRange = 2.5f;

    [Header("Timing")]
    [SerializeField] float windUpSeconds = 1.2f;
    [SerializeField] float cooldownSeconds = 2.5f;

    [Header("Visuals")]
    [SerializeField] LineRenderer beam;
    [SerializeField] Color windUpColor = new Color(1f, 0.6f, 0.1f, 0.5f);
    [SerializeField] Color firingColor = Color.red;
    [SerializeField] float windUpWidth = 0.05f;
    [SerializeField] float firingWidth = 0.2f;

    [Header("Audio")]
    [SerializeField] AudioClip windUpSound;
    [SerializeField] AudioClip firingSound;

    State state = State.Idle;
    float timer;
    Transform player;
    CollisionHandler playerCollisionHandler;
    AudioSource audioSource;
    FollowingEnemy drone;

    float EffectiveRange { get { return DifficultyRuntime.VisionRange(beamRange); } }
    float EffectiveWindUp { get { return DifficultyRuntime.BulletDelay(windUpSeconds); } }

    Transform Origin { get { return muzzle != null ? muzzle : transform; } }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        drone = GetComponentInParent<FollowingEnemy>();

        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO != null)
        {
            player = playerGO.transform;
            playerCollisionHandler = playerGO.GetComponent<CollisionHandler>();
        }

        SetBeamActive(false);
    }

    void Update()
    {
        if (player == null) return;

        if (!DifficultyRuntime.BeamEnabled)
        {
            if (state != State.Idle)
            {
                state = State.Idle;
                timer = 0f;
            }
            SetBeamActive(false);
            return;
        }

        switch (state)
        {
            case State.Idle: UpdateIdle(); break;
            case State.WindUp: UpdateWindUp(); break;
            case State.Firing: UpdateFiring(); break;
            case State.Cooldown: UpdateCooldown(); break;
        }
    }

    bool CanFire()
    {
        return drone != null && drone.IsChasing && InRange();
    }

    bool InRange()
    {
        float d = DistanceInPlane(player.position);
        return d > beamMinRange && d <= EffectiveRange;
    }

    void UpdateIdle()
    {
        if (!CanFire()) return;

        state = State.WindUp;
        timer = EffectiveWindUp;
        SetBeamActive(true);
        ApplyBeamStyle(windUpColor, windUpWidth);
        PlayOnce(windUpSound);
    }

    void UpdateWindUp()
    {
        if (!CanFire())
        {
            state = State.Idle;
            SetBeamActive(false);
            return;
        }

        DrawBeam();
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            state = State.Firing;
            ApplyBeamStyle(firingColor, firingWidth);
            PlayOnce(firingSound);
        }
    }

    void UpdateFiring()
    {
        if (!CanFire())
        {
            state = State.Cooldown;
            timer = cooldownSeconds;
            SetBeamActive(false);
            return;
        }

        DrawBeam();
        if (playerCollisionHandler != null)
        {
            playerCollisionHandler.TakeHazardHit();
        }
    }

    void UpdateCooldown()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            state = State.Idle;
        }
    }

    float DistanceInPlane(Vector3 world)
    {
        Vector3 o = Origin.position;
        return Vector2.Distance(new Vector2(o.x, o.y), new Vector2(world.x, world.y));
    }

    void DrawBeam()
    {
        if (beam == null) return;
        beam.positionCount = 2;
        beam.SetPosition(0, Origin.position);
        beam.SetPosition(1, player.position);
    }

    void ApplyBeamStyle(Color color, float width)
    {
        if (beam == null) return;
        beam.startColor = color;
        beam.endColor = color;
        beam.startWidth = width;
        beam.endWidth = width;
    }

    void SetBeamActive(bool active)
    {
        if (beam != null) beam.enabled = active;
    }

    void PlayOnce(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }

    public void ResetToSpawn()
    {
        state = State.Idle;
        timer = 0f;
        SetBeamActive(false);
    }
}
