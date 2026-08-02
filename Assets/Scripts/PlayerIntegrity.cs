using UnityEngine;

public class PlayerIntegrity : MonoBehaviour, ICheckpointable
{
    int current;
    IntegrityManager display;
    bool isTutorialScene;
    bool resolvedScene;

    public int Max { get { return DifficultyManager.MaxIntegrityFor(DifficultyManager.Current); } }
    public int Current { get { return current; } }

    private void Awake()
    {
        current = Max;
    }

    private void Start()
    {
        isTutorialScene = FindObjectOfType<TutorialManager>() != null;
        resolvedScene = true;
        Publish();
    }

    private void Publish()
    {
        if (!resolvedScene) return;

        if (display == null && !isTutorialScene)
        {
            display = FindObjectOfType<IntegrityManager>();
        }
        if (display != null)
        {
            display.SetIntegrity(current, Max);
        }
    }

    public void Refill()
    {
        current = Max;
        Publish();
    }

    public bool Absorb()
    {
        current = Mathf.Max(0, current - 1);
        Publish();
        return current > 0;
    }

    private class IntegrityMemento
    {
        public int current;
    }

    public object CaptureState()
    {
        return new IntegrityMemento { current = current };
    }

    public void RestoreState(object memento)
    {
        if (memento is IntegrityMemento integrityMemento)
        {
            current = integrityMemento.current;
            Publish();
        }
    }
}
