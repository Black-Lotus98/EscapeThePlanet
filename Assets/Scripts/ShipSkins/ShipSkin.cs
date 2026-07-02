using UnityEngine;

// One selectable player-ship skin. Skins are visual-only: the player's physics
// (rigidbody, hull + part colliders) are shared by every skin so the choice is
// purely cosmetic and levels stay balanced.
[CreateAssetMenu(fileName = "ShipSkin", menuName = "EscapeThePlanet/Ship Skin")]
public class ShipSkin : ScriptableObject
{
    public enum UnlockMode
    {
        AlwaysUnlocked,   // current release: everything free
        TotalStars,       // future: unlock at a total collected-stars milestone
        LevelsCompleted   // future: unlock after beating N levels
    }

    [Tooltip("Stable id saved to PlayerPrefs; never change it after a release.")]
    public string id;
    [Tooltip("Name shown in the hangar.")]
    public string displayName;
    [Tooltip("Factory that builds this skin's visual. Leave null for the built-in AtomRocket look (skin #0).")]
    public ShipVisualFactory factory;
    [Tooltip("Optional factory used only for the hangar preview; falls back to 'factory'. Lets skin #0 preview the baked AtomRocket.")]
    public ShipVisualFactory previewFactory;

    [Tooltip("Optional colour variants. Index 0 in the hangar is always the ship's default colours; these follow after it.")]
    public ShipColorScheme[] colorOptions;

    [Header("Unlocking (wired for future progression releases)")]
    public UnlockMode unlockMode = UnlockMode.AlwaysUnlocked;
    [Tooltip("Stars or levels required, depending on the unlock mode.")]
    public int unlockThreshold = 0;

    // Total selectable colours (1 = default only).
    public int ColorCount => 1 + (colorOptions != null ? colorOptions.Length : 0);

    // Scheme for a hangar colour index; 0 (or out of range) = default colours (null).
    public ShipColorScheme GetScheme(int index)
    {
        if (index <= 0 || colorOptions == null || index > colorOptions.Length) return null;
        return colorOptions[index - 1];
    }

    // True when this skin replaces the baked default look in-game.
    public bool HasCustomVisual => factory != null;

    public GameObject CreateVisual(Transform mount)
    {
        return factory != null ? factory.Create(mount) : null;
    }

    public GameObject CreatePreview(Transform mount)
    {
        ShipVisualFactory f = previewFactory != null ? previewFactory : factory;
        return f != null ? f.Create(mount) : null;
    }

    // Progression values are read from PlayerPrefs so the gameplay code that will
    // eventually write them ("TotalStars", "LevelsCompleted") stays decoupled.
    public bool IsUnlocked()
    {
        switch (unlockMode)
        {
            case UnlockMode.TotalStars:
                return PlayerPrefs.GetInt("TotalStars", 0) >= unlockThreshold;
            case UnlockMode.LevelsCompleted:
                return PlayerPrefs.GetInt("LevelsCompleted", 0) >= unlockThreshold;
            default:
                return true;
        }
    }
}
