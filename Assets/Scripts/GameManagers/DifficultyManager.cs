using UnityEngine;

// Global difficulty, persisted in PlayerPrefs. Drives the number of revive attempts
// granted per checkpoint:
//   Easy   -> unlimited (no Game-Over, hearts HUD hidden)
//   Medium -> 5
//   Hard   -> 3
public enum Difficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2,
}

public static class DifficultyManager
{
    private const string PrefKey = "difficulty";

    // Sentinel used across the checkpoint system to mean "unlimited attempts".
    public const int Unlimited = int.MaxValue;

    public static Difficulty Current
    {
        get { return (Difficulty)PlayerPrefs.GetInt(PrefKey, (int)Difficulty.Medium); }
        set
        {
            PlayerPrefs.SetInt(PrefKey, (int)value);
            PlayerPrefs.Save();
        }
    }

    public static int MaxAttemptsFor(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy: return Unlimited;
            case Difficulty.Medium: return 5;
            case Difficulty.Hard: return 3;
            default: return 5;
        }
    }
}
