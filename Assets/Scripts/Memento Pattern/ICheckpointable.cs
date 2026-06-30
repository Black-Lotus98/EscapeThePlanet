// Memento Pattern - Originator role.
// Any component whose state should be snapshotted when a checkpoint is reached and
// restored when the player respawns implements this interface. CaptureState returns
// an opaque memento object that is later handed back to RestoreState.
public interface ICheckpointable
{
    object CaptureState();
    void RestoreState(object memento);
}
