using UnityEngine;
using UnityEngine.SceneManagement;

public static class DifficultyRuntime
{
    const int FirstLevelBuildIndex = 1;
    const int LastLevelBuildIndex = 14;

    static Vector3 baseGravity;
    static bool gravityCaptured;

    static int escalationFrame = -1;
    static int escalationScene = int.MinValue;
    static float cachedEscalation = 1f;

    static int cachedLevelTIndex = int.MinValue;
    static float cachedLevelT;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Install()
    {
        if (!gravityCaptured)
        {
            baseGravity = Physics.gravity;
            gravityCaptured = true;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyGravity();
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    static void InstallEditorHooks()
    {
        UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(UnityEditor.PlayModeStateChange state)
    {
        if (state == UnityEditor.PlayModeStateChange.EnteredEditMode && gravityCaptured)
        {
            Physics.gravity = baseGravity;
        }
    }
#endif

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        escalationFrame = -1;
        escalationScene = int.MinValue;
        cachedLevelTIndex = int.MinValue;
        ApplyGravity();
    }

    public static void ApplyGravity()
    {
        if (!gravityCaptured) return;
        if (!Application.isPlaying) return;
        DifficultyProfile profile = DifficultyManager.CurrentProfile;
        Physics.gravity = baseGravity * (profile != null ? profile.gravityScale : 1f);
    }

    static float LevelT
    {
        get
        {
            int index = SceneManager.GetActiveScene().buildIndex;
            if (index != cachedLevelTIndex)
            {
                cachedLevelTIndex = index;
                cachedLevelT = (index < FirstLevelBuildIndex || index > LastLevelBuildIndex)
                    ? 0f
                    : Mathf.InverseLerp(FirstLevelBuildIndex, LastLevelBuildIndex, index);
            }
            return cachedLevelT;
        }
    }

    static float Eval(AnimationCurve curve, float t)
    {
        if (curve == null || curve.length == 0) return 1f;
        return curve.Evaluate(t);
    }

    public static float Escalation
    {
        get
        {
            int sceneHandle = SceneManager.GetActiveScene().handle;
            if (escalationFrame == Time.frameCount && escalationScene == sceneHandle) return cachedEscalation;
            escalationFrame = Time.frameCount;
            escalationScene = sceneHandle;

            DifficultyProfile profile = DifficultyManager.CurrentProfile;
            if (profile == null)
            {
                cachedEscalation = 1f;
                return cachedEscalation;
            }

            float progressT = Mathf.Clamp01(Time.timeSinceLevelLoad / Mathf.Max(0.01f, profile.inLevelRampSeconds));
            float combined = Eval(profile.levelIndexCurve, LevelT) * Eval(profile.inLevelCurve, progressT);
            cachedEscalation = Mathf.Clamp(combined, 0.01f, 10f);
            return cachedEscalation;
        }
    }

    public static float ChaseSpeed(float authored)
    {
        DifficultyProfile profile = DifficultyManager.CurrentProfile;
        return profile == null ? authored : authored * profile.chaseSpeedScale * Escalation;
    }

    public static float VisionRange(float authored)
    {
        DifficultyProfile profile = DifficultyManager.CurrentProfile;
        return profile == null ? authored : authored * profile.visionRangeScale * Escalation;
    }

    public static float VisionAngle(float authored)
    {
        DifficultyProfile profile = DifficultyManager.CurrentProfile;
        return profile == null ? authored : authored * profile.visionAngleScale * Escalation;
    }

    public static float BulletDelay(float authored)
    {
        DifficultyProfile profile = DifficultyManager.CurrentProfile;
        return profile == null ? authored : authored * profile.bulletDelayScale / Escalation;
    }

    public static float BulletSpeed(float authored)
    {
        DifficultyProfile profile = DifficultyManager.CurrentProfile;
        return profile == null ? authored : authored * profile.bulletSpeedScale * Escalation;
    }

    public static float FuelCapacity(float authored)
    {
        DifficultyProfile profile = DifficultyManager.CurrentProfile;
        return profile == null ? authored : authored * profile.fuelCapacityScale / Escalation;
    }

    public static float FuelConsumption(float authored)
    {
        DifficultyProfile profile = DifficultyManager.CurrentProfile;
        return profile == null ? authored : authored * profile.fuelConsumptionScale * Escalation;
    }

    public static float ShieldTime(float authored)
    {
        DifficultyProfile profile = DifficultyManager.CurrentProfile;
        return profile == null ? authored : authored * profile.shieldTimeScale / Escalation;
    }

    public static float ThrustScale
    {
        get
        {
            DifficultyProfile profile = DifficultyManager.CurrentProfile;
            return profile != null ? profile.thrustScale : 1f;
        }
    }

    public static bool BeamEnabled
    {
        get
        {
            DifficultyProfile profile = DifficultyManager.CurrentProfile;
            return profile != null && profile.beamEnabled;
        }
    }

    public static bool ShieldDisabled
    {
        get
        {
            DifficultyProfile profile = DifficultyManager.CurrentProfile;
            return profile != null && profile.disableShield;
        }
    }
}
