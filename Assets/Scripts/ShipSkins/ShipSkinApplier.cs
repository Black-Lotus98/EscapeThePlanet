using System.Collections.Generic;
using UnityEngine;

// Applies the hangar-selected skin to the player at level start.
// Skin #0 (AtomRocket) keeps the renderers baked into the prefab; any other
// skin hides those and instantiates its visual prefab under the mount.
// Colliders are never touched, so every skin flies with identical physics.
public class ShipSkinApplier : MonoBehaviour
{
    [Tooltip("Empty child the skin visual is instantiated under.")]
    [SerializeField] Transform visualMount;
    [Tooltip("The prefab's built-in AtomRocket renderers, hidden when another skin is active.")]
    [SerializeField] List<Renderer> defaultRenderers = new List<Renderer>();

    void Start()
    {
        var catalog = ShipCatalog.Load();
        if (catalog == null)
        {
            Debug.LogWarning("ShipSkinApplier: no ShipCatalog in Resources; keeping default look.", this);
            return;
        }

        ShipSkin skin = ShipSelection.Resolve(catalog);
        if (skin == null || !skin.HasCustomVisual) return; // default AtomRocket look

        foreach (var rend in defaultRenderers)
            if (rend != null) rend.enabled = false;

        GameObject visual = skin.CreateVisual(visualMount != null ? visualMount : transform);
        ShipColorScheme scheme = skin.GetScheme(ShipSelection.GetColorIndex(skin));
        if (visual != null && scheme != null) scheme.ApplyTo(visual);
    }
}
