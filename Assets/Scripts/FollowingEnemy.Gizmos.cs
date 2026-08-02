#if UNITY_EDITOR
using UnityEngine;

public partial class FollowingEnemy
{
    void OnDrawGizmosSelected()
    {
        Vector3 f = (Application.isPlaying && facingDir.sqrMagnitude > 0.0001f) ? facingDir : Vector3.right;
        float range = Application.isPlaying ? EffectiveVisionRange : visionRange;
        float angle = Application.isPlaying ? EffectiveVisionAngle : visionAngle;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, (Quaternion.Euler(0f, 0f, angle * 0.5f) * f) * range);
        Gizmos.DrawRay(transform.position, (Quaternion.Euler(0f, 0f, -angle * 0.5f) * f) * range);
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
#endif
