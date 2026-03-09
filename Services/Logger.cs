namespace SmartHomeHub.Services;

/// <summary>
/// Singleton — en gemensam logger-instans som alla delar använder.
/// Thread-safe via Lazy<T>.
/// </summary>
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance = new(() => new Logger());

    public static Logger Instance => _instance.Value;

    private Logger() { }

    public void Log(string message)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"  [LOG {DateTime.Now:HH:mm:ss}] {message}");
        Console.ResetColor();
    }
}
