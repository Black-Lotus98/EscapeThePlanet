using UnityEngine;

// Purely cosmetic idle motion for the seeker drone body:
//   - gently bobs this transform up and down (local Y hover)
//   - continuously spins the blade ring
// Gameplay transforms (root position, colliders, attack range) are untouched.
public class SeekerDroneVisuals : MonoBehaviour
{
    [Header("Hover Bob")]
    [Tooltip("Bob amplitude in local units (the root's scale multiplies this).")]
    [SerializeField] float bobAmplitude = 0.012f;
    [Tooltip("Bob cycles per second.")]
    [SerializeField] float bobFrequency = 1.6f;

    [Header("Blade Ring")]
    [Tooltip("The blade ring, spun around its local Z axis.")]
    [SerializeField] Transform spinRing;
    [Tooltip("Spin speed in degrees per second.")]
    [SerializeField] float spinSpeed = 240f;

    Vector3 restLocalPos;
    float phase;

    void Start()
    {
        restLocalPos = transform.localPosition;
        phase = Random.Range(0f, Mathf.PI * 2f); // desync drones that share a level
    }

    void Update()
    {
        float bob = Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f + phase) * bobAmplitude;
        transform.localPosition = restLocalPos + Vector3.up * bob;

        if (spinRing != null)
            spinRing.Rotate(0f, 0f, spinSpeed * Time.deltaTime, Space.Self);
    }
}
