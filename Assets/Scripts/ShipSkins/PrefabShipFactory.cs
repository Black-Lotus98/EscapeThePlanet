using UnityEngine;

// Concrete factory: the simplest product — instantiate a visual prefab as-is.
[CreateAssetMenu(fileName = "PrefabShipFactory", menuName = "EscapeThePlanet/Factories/Prefab Ship")]
public class PrefabShipFactory : ShipVisualFactory
{
    [Tooltip("Visual-only prefab (no gameplay components).")]
    [SerializeField] GameObject visualPrefab;

    public override GameObject Create(Transform mount)
    {
        if (visualPrefab == null)
        {
            Debug.LogWarning("PrefabShipFactory '" + name + "' has no prefab assigned.", this);
            return null;
        }
        return Instantiate(visualPrefab, mount);
    }
}
