using SmartHomeHub.Commands;
using SmartHomeHub.Devices;
using SmartHomeHub.Interfaces;
using SmartHomeHub.Observers;
using SmartHomeHub.Services;
using SmartHomeHub.Strategies;

namespace SmartHomeHub;

public class SmartHomeFacade
{
    private readonly List<IDevice> _devices = [];
    private readonly CommandInvoker _invoker = new();
    private readonly DashboardObserver _dashboard = new();
    private readonly LoggerObserver _loggerObserver = new();
    private readonly AuditObserver _audit = new();
    private IModeStrategy _mode = new NormalMode();

    public void AddDevice(IDevice device)
    {
        _devices.Add(device);

        if (device is DeviceBase observable)
        {
            observable.Subscribe(_dashboard);
            observable.Subscribe(_loggerObserver);
            observable.Subscribe(_audit);
        }

        Logger.Instance.Log($"Device added: {device.Name}");
    }

    public void SetMode(IModeStrategy mode)
    {
        _mode = mode;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  ★ Mode changed to: {_mode.ModeName}");
        Console.ResetColor();
        Logger.Instance.Log($"Mode set to {_mode.ModeName}");
    }

    public void RunCommand(ICommand command)
    {
        if (!_mode.AllowCommand(command))
            return;

        if (command is SetTemperatureCommand tempCmd)
        {
            var maxTemp = _mode.GetMaxTemperature();
            Logger.Instance.Log($"Mode {_mode.ModeName} allows max {maxTemp}°C");
        }

        _invoker.ExecuteSingle(command);
    }

    public void QueueCommand(ICommand command)
    {
        if (_mode.AllowCommand(command))
            _invoker.Enqueue(command);
    }

    public void ExecuteQueue() => _invoker.ExecuteAll();

    public void UndoLast() => _invoker.UndoLast();

    public void ReplayLast(int count = 5) => _invoker.ReplayLast(count);

    public void MorningRoutine()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n  ☀ Running Morning Routine...");
        Console.ResetColor();

        foreach (var device in _devices)
        {
            if (device is Lamp lamp)
                RunCommand(new TurnOnCommand(lamp));
            else if (device is Thermostat thermo)
                RunCommand(new SetTemperatureCommand(thermo, 22));
            else if (device is DoorLock doorLock)
                RunCommand(new LockDoorCommand(doorLock, false));
        }
    }

    public void GoodNightRoutine()
    {
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("\n  🌙 Running Good Night Routine...");
        Console.ResetColor();

        foreach (var device in _devices)
        {
            if (device is Lamp lamp)
                RunCommand(new TurnOffCommand(lamp));
            else if (device is Thermostat thermo)
                RunCommand(new SetTemperatureCommand(thermo, 18));
            else if (device is DoorLock doorLock)
                RunCommand(new LockDoorCommand(doorLock, true));
        }
    }

    public void BatchToggleLamps(bool on)
    {
        if (!_mode.AllowBatchOperations())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠ Batch operations not allowed in {_mode.ModeName} mode");
            Console.ResetColor();
            return;
        }

        foreach (var device in _devices.OfType<Lamp>())
            RunCommand(on ? new TurnOnCommand(device) : new TurnOffCommand(device));
    }

    public void ShowStatus()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"\n  ═══ Smart Home Status ({_mode.ModeName}) ═══");
        foreach (var device in _devices)
            Console.WriteLine($"    {device.GetStatus()}");
        Console.ResetColor();
    }

    public void ShowCommandHistory() => _invoker.PrintHistory();

    public IReadOnlyList<string> GetAuditTrail() => _audit.AuditTrail;
}
