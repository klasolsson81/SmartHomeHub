using SmartHomeHub.Devices;
using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Commands;

public class SetTemperatureCommand : ICommand
{
    private readonly Thermostat _thermostat;
    private readonly int _newTemp;
    private int _previousTemp;

    public int NewTemp => _newTemp;
    public string Description => $"SetTemperature({_thermostat.Name}, {_newTemp}°C)";

    public SetTemperatureCommand(Thermostat thermostat, int newTemp)
    {
        _thermostat = thermostat;
        _newTemp = newTemp;
    }

    public void Execute()
    {
        _previousTemp = _thermostat.Temperature;
        _thermostat.SetTemperature(_newTemp);
    }

    public void Undo() => _thermostat.SetTemperature(_previousTemp);
}
