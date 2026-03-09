namespace SmartHomeHub.Services;

/// <summary>
/// Singleton — en gemensam logger-instans som alla delar använder.
/// Thread-safe via Lazy&lt;T&gt;.
/// </summary>
public sealed class Logger
{
    private static readonly Lazy<Logger> _instance = new(() => new Logger());
    private readonly List<string> _logs = [];
    private bool _suppressOutput;

    public static Logger Instance => _instance.Value;

    public IReadOnlyList<string> Logs => _logs;

    private Logger() { }

    public void SuppressOutput(bool suppress) => _suppressOutput = suppress;

    public void Log(string message)
    {
        var entry = $"[LOG {DateTime.Now:HH:mm:ss}] {message}";
        _logs.Add(entry);

        if (!_suppressOutput)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  {entry}");
            Console.ResetColor();
        }
    }
}
