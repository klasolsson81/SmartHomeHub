using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Services;

public class CommandInvoker
{
    private readonly Queue<ICommand> _queue = new();
    private readonly List<ICommand> _history = [];

    public void Enqueue(ICommand command)
    {
        _queue.Enqueue(command);
        Logger.Instance.Log($"Queued: {command.Description}");
    }

    public void ExecuteAll()
    {
        while (_queue.Count > 0)
        {
            var cmd = _queue.Dequeue();
            cmd.Execute();
            _history.Add(cmd);
            Logger.Instance.Log($"Executed: {cmd.Description}");
        }
    }

    public void ExecuteSingle(ICommand command)
    {
        command.Execute();
        _history.Add(command);
        Logger.Instance.Log($"Executed: {command.Description}");
    }

    public void UndoLast()
    {
        if (_history.Count == 0)
        {
            Console.WriteLine("  Nothing to undo.");
            return;
        }

        var last = _history[^1];
        _history.RemoveAt(_history.Count - 1);
        last.Undo();
        Logger.Instance.Log($"Undo: {last.Description}");
    }

    public void ReplayLast(int count = 5)
    {
        var toReplay = _history.TakeLast(Math.Min(count, _history.Count)).ToList();
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"\n  ↻ Replaying last {toReplay.Count} command(s)...");
        Console.ResetColor();

        foreach (var cmd in toReplay)
        {
            cmd.Execute();
            Logger.Instance.Log($"Replayed: {cmd.Description}");
        }
    }

    public void PrintHistory()
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("\n  Command History:");
        for (int i = 0; i < _history.Count; i++)
            Console.WriteLine($"    {i + 1}. {_history[i].Description}");
        Console.ResetColor();
    }
}
