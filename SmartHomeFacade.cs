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

    public string CurrentModeName => _mode.ModeName;
    public IReadOnlyList<IDevice> Devices => _devices;

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
        Logger.Instance.Log($"Mode set to {_mode.ModeName}");
    }

    public int GetMaxTemperature() => _mode.GetMaxTemperature();
    public bool AllowsBatch() => _mode.AllowBatchOperations();

    public CommandResult RunCommand(ICommand command)
    {
        if (!_mode.AllowCommand(command))
            return CommandResult.Blocked($"Blocked by {_mode.ModeName}");

        if (command is SetTemperatureCommand tempCmd && tempCmd.NewTemp > _mode.GetMaxTemperature())
            return CommandResult.Blocked($"Temperature exceeds max {_mode.GetMaxTemperature()}°C in {_mode.ModeName}");

        _invoker.ExecuteSingle(command);
        return CommandResult.Ok();
    }

    public void QueueCommand(ICommand command)
    {
        if (_mode.AllowCommand(command))
            _invoker.Enqueue(command);
    }

    public void ExecuteQueue() => _invoker.ExecuteAll();
    public bool UndoLast() => _invoker.UndoLast();
    public int ReplayLast(int count = 5) => _invoker.ReplayLast(count);

    public void MorningRoutine()
    {
        Logger.Instance.Log("Running Morning Routine");
        foreach (var device in _devices)
        {
            if (device is Lamp lamp) RunCommand(new TurnOnCommand(lamp));
            else if (device is Thermostat t) RunCommand(new SetTemperatureCommand(t, 22));
            else if (device is DoorLock d) RunCommand(new LockDoorCommand(d, false));
        }
    }

    public void GoodNightRoutine()
    {
        Logger.Instance.Log("Running Good Night Routine");
        foreach (var device in _devices)
        {
            if (device is Lamp lamp) RunCommand(new TurnOffCommand(lamp));
            else if (device is Thermostat t) RunCommand(new SetTemperatureCommand(t, 18));
            else if (device is DoorLock d) RunCommand(new LockDoorCommand(d, true));
        }
    }

    public CommandResult BatchToggleLamps(bool on)
    {
        if (!_mode.AllowBatchOperations())
            return CommandResult.Blocked($"Batch not allowed in {_mode.ModeName}");

        foreach (var lamp in _devices.OfType<Lamp>())
            RunCommand(on ? new TurnOnCommand(lamp) : new TurnOffCommand(lamp));

        return CommandResult.Ok();
    }

    public IReadOnlyList<string> GetCommandHistory() => _invoker.GetHistory();
    public IReadOnlyList<string> GetAuditTrail() => _audit.AuditTrail;
}

public record CommandResult(bool Success, string? Message)
{
    public static CommandResult Ok() => new(true, null);
    public static CommandResult Blocked(string reason) => new(false, reason);
}
