// Memento Pattern - support role for hazards.
// Hazards (enemies, oscillating obstacles, cannons) are NOT snapshotted per-checkpoint;
// instead they reset to their authored spawn state on respawn. Implement this to be
// reset by the CheckpointManager when the player respawns.
public interface IRespawnResettable
{
    void ResetToSpawn();
}
