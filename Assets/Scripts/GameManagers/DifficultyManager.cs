using UnityEngine;

public enum Difficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2,
}

public static class DifficultyManager
{
    private const string PrefKey = "difficulty";

    public const int Unlimited = int.MaxValue;

    static DifficultyProfile cachedProfile;
    static Difficulty cachedFor;
    static bool hasCachedProfile;

    public static Difficulty Current
    {
        get { return (Difficulty)PlayerPrefs.GetInt(PrefKey, (int)Difficulty.Medium); }
        set
        {
            PlayerPrefs.SetInt(PrefKey, (int)value);
            PlayerPrefs.Save();
            cachedProfile = null;
            hasCachedProfile = false;
            DifficultyRuntime.ApplyGravity();
        }
    }

    public static DifficultyProfile CurrentProfile
    {
        get { return ProfileFor(Current); }
    }

    public static DifficultyProfile ProfileFor(Difficulty difficulty)
    {
        if (hasCachedProfile && cachedFor == difficulty)
        {
            return cachedProfile;
        }

        DifficultyCatalog catalog = DifficultyCatalog.Load();
        DifficultyProfile resolved = catalog != null ? catalog.Find(difficulty) : null;
        if (resolved == null)
        {
            return null;
        }

        cachedProfile = resolved;
        cachedFor = difficulty;
        hasCachedProfile = true;
        return cachedProfile;
    }

    public static int MaxAttemptsFor(Difficulty difficulty)
    {
        DifficultyProfile profile = ProfileFor(difficulty);
        if (profile == null)
        {
            switch (difficulty)
            {
                case Difficulty.Easy: return Unlimited;
                case Difficulty.Medium: return 5;
                case Difficulty.Hard: return 3;
                default: return 5;
            }
        }
        return profile.unlimitedAttempts ? Unlimited : profile.maxAttempts;
    }

    public static int MaxIntegrityFor(Difficulty difficulty)
    {
        DifficultyProfile profile = ProfileFor(difficulty);
        if (profile == null)
        {
            switch (difficulty)
            {
                case Difficulty.Easy: return 3;
                case Difficulty.Medium: return 2;
                case Difficulty.Hard: return 1;
                default: return 2;
            }
        }
        return Mathf.Max(1, profile.maxIntegrity);
    }

    public static float HitImmunityFor(Difficulty difficulty)
    {
        DifficultyProfile profile = ProfileFor(difficulty);
        return profile == null ? 1f : Mathf.Max(0.01f, profile.hitImmunitySeconds);
    }
}
