
public interface IUIObserver<T> where T : UIManager
{
    void OnStateChange(T manager, UIState state);
}


