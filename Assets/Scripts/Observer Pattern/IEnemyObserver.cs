// Observer Pattern - observer side.
// Implemented by objects that react to a FollowingEnemy's patrol movement
// (e.g. obstacles that pull aside so they don't block the patrolling enemy).
public interface IEnemyObserver
{
    void OnEnemyMoved(FollowingEnemy enemy);
}
