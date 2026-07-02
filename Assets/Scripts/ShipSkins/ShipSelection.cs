using UnityEngine;

// Persists which ship skin the player picked in the hangar (PlayerPrefs).
public static class ShipSelection
{
    const string PrefKey = "SelectedShipId";

    public static string SelectedId
    {
        get => PlayerPrefs.GetString(PrefKey, string.Empty);
        set
        {
            PlayerPrefs.SetString(PrefKey, value);
            PlayerPrefs.Save();
        }
    }

    // Colour choice is remembered per ship, so switching ships keeps each
    // ship's favourite paint job.
    public static int GetColorIndex(ShipSkin skin)
    {
        if (skin == null) return 0;
        int i = PlayerPrefs.GetInt("ShipColor_" + skin.id, 0);
        return Mathf.Clamp(i, 0, skin.ColorCount - 1);
    }

    public static void SetColorIndex(ShipSkin skin, int index)
    {
        if (skin == null) return;
        PlayerPrefs.SetInt("ShipColor_" + skin.id, Mathf.Clamp(index, 0, skin.ColorCount - 1));
        PlayerPrefs.Save();
    }

    // Resolves the saved selection against the catalog; falls back to skin #0
    // (and to the default look if the saved skin is locked or was removed).
    public static ShipSkin Resolve(ShipCatalog catalog)
    {
        if (catalog == null || catalog.skins.Count == 0) return null;

        ShipSkin saved = catalog.FindById(SelectedId);
        if (saved != null && saved.IsUnlocked()) return saved;
        return catalog.skins[0];
    }
}
