using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    [SerializeField] AnimationClip animClip;

    void Update()
    {
        Destroy(gameObject, animClip.length);
    }
}
