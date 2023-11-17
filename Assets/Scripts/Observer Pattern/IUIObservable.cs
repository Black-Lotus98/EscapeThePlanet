
public interface IUIObservable<T> where T : UIManager
{
    void NotifyObservers(UIState state);

    void AddObserver(IUIObserver<T> anObserver);

    void RemoveObserver(IUIObserver<T> anObserver);
}
