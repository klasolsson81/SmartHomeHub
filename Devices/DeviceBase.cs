using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Devices;

/// <summary>
/// Abstrakt basklass för alla smarta enheter.
/// Implementerar Observer-subjekt (subscribe/notify) och gemensam on/off-logik.
/// </summary>
public abstract class DeviceBase : IDevice, IObservable
{
    private readonly List<IObserver> _observers = [];

    public string Name { get; }
    public bool IsOn { get; protected set; }

    protected DeviceBase(string name)
    {
        Name = name;
    }

    public void Subscribe(IObserver observer) => _observers.Add(observer);
    public void Unsubscribe(IObserver observer) => _observers.Remove(observer);

    protected void NotifyObservers(string eventDescription)
    {
        foreach (var observer in _observers)
            observer.Update(Name, eventDescription);
    }

    public virtual void TurnOn()
    {
        IsOn = true;
        NotifyObservers("turned ON");
    }

    public virtual void TurnOff()
    {
        IsOn = false;
        NotifyObservers("turned OFF");
    }

    public abstract string GetStatus();
}
