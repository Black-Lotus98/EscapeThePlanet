// This is feature should be added to easier difficulty of the game, it still under development

using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    [SerializeField] AnimationClip animClip;

    void Update()
    {
        Destroy(gameObject, animClip.length);
    }
}
