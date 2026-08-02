using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyCatalog", menuName = "EscapeThePlanet/Difficulty Catalog")]
public class DifficultyCatalog : ScriptableObject
{
    public const string ResourcesPath = "DifficultyCatalog";

    public List<DifficultyProfile> profiles = new List<DifficultyProfile>();

    static DifficultyCatalog cached;
    public static DifficultyCatalog Load()
    {
        if (cached == null)
            cached = Resources.Load<DifficultyCatalog>(ResourcesPath);
        return cached;
    }

    public DifficultyProfile Find(Difficulty difficulty)
    {
        foreach (var profile in profiles)
            if (profile != null && profile.difficulty == difficulty)
                return profile;
        return null;
    }
}
