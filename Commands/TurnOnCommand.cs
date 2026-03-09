using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Commands;

public class TurnOnCommand : ICommand
{
    private readonly IDevice _device;

    public string Description => $"TurnOn({_device.Name})";

    public TurnOnCommand(IDevice device) => _device = device;

    public void Execute() => _device.TurnOn();
    public void Undo() => _device.TurnOff();
}
