// This is feature should be added to easier difficulty of the game, it still under development

using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    [SerializeField] AnimationClip animClip;

    void Start()
    {
        if (animClip == null)
        {
            Debug.LogError("CameraAnimator: animClip is not assigned!", this);
            Destroy(gameObject);
            return;
        }
        Destroy(gameObject, animClip.length);
    }
}
