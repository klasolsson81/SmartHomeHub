using SmartHomeHub.Interfaces;
using SmartHomeHub.Services;

namespace SmartHomeHub.Observers;

/// <summary>
/// Observer — vidarebefordrar enhetshändelser till Logger-singletonen.
/// </summary>
public class LoggerObserver : IObserver
{
    public void Update(string deviceName, string eventDescription)
    {
        Logger.Instance.Log($"{deviceName} → {eventDescription}");
    }
}
