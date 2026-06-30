using UnityEngine;

// Mirrors a star collectible's presence. Stars now DEACTIVATE (not Destroy) when
// collected so checkpoints can restore them, so this trigger watches the star's active
// state instead of a null reference and deactivates with it (and comes back on respawn).
public class StarSoundTrigger : MonoBehaviour, ICheckpointable
{
    [SerializeField] GameObject StarCollectable;

    void Update()
    {
        // When the star has been collected (deactivated or destroyed), retire this trigger.
        if (StarCollectable == null || !StarCollectable.activeSelf)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public object CaptureState()
    {
        return gameObject.activeSelf;
    }

    public void RestoreState(object memento)
    {
        if (memento is bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
