using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Observers;

public class AuditObserver : IObserver
{
    private readonly List<string> _auditTrail = [];

    public IReadOnlyList<string> AuditTrail => _auditTrail;

    public void Update(string deviceName, string eventDescription)
    {
        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {deviceName}: {eventDescription}";
        _auditTrail.Add(entry);
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"  [AUDIT] {entry}");
        Console.ResetColor();
    }
}
