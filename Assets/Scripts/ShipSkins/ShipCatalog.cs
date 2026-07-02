using System.Collections.Generic;
using UnityEngine;

// The ordered list of all selectable ship skins. Lives in Resources so the
// player prefab and the main-menu hangar can both load it without scene wiring.
[CreateAssetMenu(fileName = "ShipCatalog", menuName = "EscapeThePlanet/Ship Catalog")]
public class ShipCatalog : ScriptableObject
{
    public const string ResourcesPath = "ShipCatalog";

    [Tooltip("Skin #0 is the default (AtomRocket).")]
    public List<ShipSkin> skins = new List<ShipSkin>();

    static ShipCatalog cached;
    public static ShipCatalog Load()
    {
        if (cached == null)
            cached = Resources.Load<ShipCatalog>(ResourcesPath);
        return cached;
    }

    public ShipSkin FindById(string id)
    {
        foreach (var skin in skins)
            if (skin != null && skin.id == id)
                return skin;
        return null;
    }

    public int IndexOf(ShipSkin skin)
    {
        return skins.IndexOf(skin);
    }
}
