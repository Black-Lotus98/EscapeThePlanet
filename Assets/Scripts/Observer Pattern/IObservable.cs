public interface IObservable
{
    void NotifyObservers(UIState state);

    void AddObserver(IPlayerObserver anObserver);

    void RemoveObserver(IPlayerObserver anObserver);
}
