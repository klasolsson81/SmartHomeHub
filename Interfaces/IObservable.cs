namespace SmartHomeHub.Interfaces;

/// <summary>
/// Observer-mönstret — subjekt som observers kan prenumerera på.
/// </summary>
public interface IObservable
{
    void Subscribe(IObserver observer);
    void Unsubscribe(IObserver observer);
}
