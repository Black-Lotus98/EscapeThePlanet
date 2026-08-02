using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyProfile", menuName = "EscapeThePlanet/Difficulty Profile")]
public class DifficultyProfile : ScriptableObject
{
    public Difficulty difficulty = Difficulty.Medium;

    [Header("Survivability")]
    public int maxIntegrity = 2;
    public int maxAttempts = 5;
    public bool unlimitedAttempts = false;
    public float hitImmunitySeconds = 1f;

    [Header("Escalation")]
    public AnimationCurve levelIndexCurve = AnimationCurve.Constant(0f, 1f, 1f);
    public AnimationCurve inLevelCurve = AnimationCurve.Constant(0f, 1f, 1f);
    public float inLevelRampSeconds = 120f;

    [Header("Hazards")]
    public float chaseSpeedScale = 1f;
    public float visionRangeScale = 1f;
    public float visionAngleScale = 1f;
    public float bulletDelayScale = 1f;
    public float bulletSpeedScale = 1f;
    public bool beamEnabled = false;

    [Header("Resources")]
    public float fuelCapacityScale = 1f;
    public float fuelConsumptionScale = 1f;
    public float shieldTimeScale = 1f;
    public bool disableShield = false;

    [Header("Flight")]
    public float thrustScale = 1f;
    public float gravityScale = 1f;

    private void OnValidate()
    {
        DifficultyRuntime.ApplyGravity();
    }
}
