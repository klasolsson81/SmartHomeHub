using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Observers;

/// <summary>
/// Observer — samlar notifikationer som visas i StatusDisplay.
/// </summary>
public class DashboardObserver : IObserver
{
    private readonly List<string> _notifications = [];

    public IReadOnlyList<string> Notifications => _notifications;

    public void Update(string deviceName, string eventDescription)
    {
        _notifications.Add($"{deviceName} → {eventDescription}");
    }
}
