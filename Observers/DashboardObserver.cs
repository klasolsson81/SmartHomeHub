using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Observers;

public class DashboardObserver : IObserver
{
    public void Update(string deviceName, string eventDescription)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  [DASHBOARD] {deviceName} → {eventDescription}");
        Console.ResetColor();
    }
}
