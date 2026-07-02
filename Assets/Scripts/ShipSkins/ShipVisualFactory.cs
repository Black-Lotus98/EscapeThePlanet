using UnityEngine;

// Factory Method seam for ship visuals: callers (ShipSkinApplier, the hangar)
// ask a factory for a visual and never know how it gets built. New ways of
// building ships (tinted variants, seasonal skins, procedural effects) are new
// factory assets — no changes to the callers.
public abstract class ShipVisualFactory : ScriptableObject
{
    // Build the ship visual as a child of 'mount' and return it.
    public abstract GameObject Create(Transform mount);
}
