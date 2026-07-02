using System;
using UnityEngine;

// A coherent family of replacement materials (the 'product family' of the
// abstract-factory idea, expressed as data): one scheme recolors ANY ship
// consistently by swapping its shared materials.
[CreateAssetMenu(fileName = "ShipColorScheme", menuName = "EscapeThePlanet/Ship Color Scheme")]
public class ShipColorScheme : ScriptableObject
{
    [Serializable]
    public struct MaterialSwap
    {
        [Tooltip("Base ship material to replace (e.g. Ship_White).")]
        public Material from;
        [Tooltip("Scheme material to use instead (e.g. Ship_Crimson).")]
        public Material to;
    }

    [Tooltip("Name shown in the hangar (e.g. Crimson).")]
    public string displayName;
    [Tooltip("Colour of this scheme's swatch button in the hangar.")]
    public Color swatchColor = Color.white;

    [SerializeField] MaterialSwap[] swaps;

    // Replace matching shared materials on every renderer under 'visual'.
    public void ApplyTo(GameObject visual)
    {
        if (visual == null || swaps == null) return;

        foreach (var rend in visual.GetComponentsInChildren<Renderer>(true))
        {
            var mats = rend.sharedMaterials;
            bool changed = false;
            for (int i = 0; i < mats.Length; i++)
            {
                foreach (var swap in swaps)
                {
                    if (mats[i] == swap.from && swap.to != null)
                    {
                        mats[i] = swap.to;
                        changed = true;
                    }
                }
            }
            if (changed) rend.sharedMaterials = mats;
        }
    }
}
