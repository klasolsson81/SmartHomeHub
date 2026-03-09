using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Commands;

public class TurnOffCommand : ICommand
{
    private readonly IDevice _device;

    public string Description => $"TurnOff({_device.Name})";

    public TurnOffCommand(IDevice device) => _device = device;

    public void Execute() => _device.TurnOff();
    public void Undo() => _device.TurnOn();
}
