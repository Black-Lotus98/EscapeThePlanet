using UnityEngine;

// Visual feedback for a checkpoint: tints its particle effect (and optionally a mesh
// renderer) when it becomes the active checkpoint. CheckpointManager calls Activate()
// once, on first activation. By default it keeps the authored colors until activated;
// enable Override Inactive Color to force a starting color too.
public class CheckpointVisual : MonoBehaviour
{
    [Tooltip("Particle system to recolor on activation. Auto-found in children if left empty.")]
    [SerializeField] private ParticleSystem targetParticles;
    [Tooltip("Optional mesh renderer to also tint. Auto-found if left empty.")]
    [SerializeField] private Renderer targetRenderer;

    [Tooltip("Color applied once this checkpoint has been activated.")]
    [SerializeField] private Color activatedColor = new Color(0.1f, 0.3f, 1f, 1f);

    [Tooltip("If true, force the inactive color below at start instead of keeping the authored color.")]
    [SerializeField] private bool overrideInactiveColor = false;
    [SerializeField] private Color inactiveColor = new Color(0.9f, 0.1f, 0.1f, 1f);

    private bool activated = false;

    private void Awake()
    {
        if (targetParticles == null) targetParticles = GetComponentInChildren<ParticleSystem>(true);
        if (targetRenderer == null) targetRenderer = GetComponent<MeshRenderer>();

        if (overrideInactiveColor)
        {
            ApplyColor(inactiveColor);
        }
    }

    public void Activate()
    {
        if (activated) return;
        activated = true;
        ApplyColor(activatedColor);
    }

    private void ApplyColor(Color color)
    {
        // Recolor the particle effect (affects newly emitted particles).
        if (targetParticles != null)
        {
            ParticleSystem.MainModule main = targetParticles.main;
            main.startColor = color;
        }

        // Optionally recolor a mesh too.
        if (targetRenderer != null && targetRenderer.material != null)
        {
            targetRenderer.material.color = color;
        }
    }
}
