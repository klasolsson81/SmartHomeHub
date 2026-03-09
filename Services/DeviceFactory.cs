using SmartHomeHub.Devices;
using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Services;

/// <summary>
/// Factory Method — skapar enheter baserat på typ-sträng.
/// Gör det enkelt att lägga till nya enhetstyper utan att ändra anropande kod.
/// </summary>
public static class DeviceFactory
{
    public static IDevice Create(string type, string name)
    {
        return type.ToLower() switch
        {
            "lamp" => new Lamp(name),
            "thermostat" => new Thermostat(name),
            "doorlock" => new DoorLock(name),
            _ => throw new ArgumentException($"Unknown device type: {type}")
        };
    }
}
