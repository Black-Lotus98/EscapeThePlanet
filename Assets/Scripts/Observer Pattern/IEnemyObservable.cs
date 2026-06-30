// Observer Pattern - subject side.
// A FollowingEnemy that broadcasts its patrol movement to interested observers
// (obstacles) so they can yield to it without each one polling the scene.
public interface IEnemyObservable
{
    void AddObserver(IEnemyObserver observer);

    void RemoveObserver(IEnemyObserver observer);

    void NotifyObservers();
}
