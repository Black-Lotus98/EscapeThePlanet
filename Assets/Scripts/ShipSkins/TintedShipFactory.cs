using UnityEngine;

// Concrete factory: instantiate a base visual prefab, then apply a colour
// scheme. A ship-with-colour-variant is one small asset pointing at an
// existing prefab + an existing scheme — no duplicate prefabs, no new code.
[CreateAssetMenu(fileName = "TintedShipFactory", menuName = "EscapeThePlanet/Factories/Tinted Ship")]
public class TintedShipFactory : ShipVisualFactory
{
    [Tooltip("Base visual-only prefab shared with the untinted skin.")]
    [SerializeField] GameObject visualPrefab;
    [Tooltip("Material family applied on top of the base prefab.")]
    [SerializeField] ShipColorScheme scheme;

    public override GameObject Create(Transform mount)
    {
        if (visualPrefab == null)
        {
            Debug.LogWarning("TintedShipFactory '" + name + "' has no prefab assigned.", this);
            return null;
        }
        GameObject visual = Instantiate(visualPrefab, mount);
        if (scheme != null) scheme.ApplyTo(visual);
        return visual;
    }
}
