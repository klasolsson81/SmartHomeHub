namespace SmartHomeHub.Devices;

public class Thermostat : DeviceBase
{
    public int Temperature { get; private set; } = 20;

    public Thermostat(string name = "Thermostat") : base(name) { }

    public void SetTemperature(int temp)
    {
        var old = Temperature;
        Temperature = temp;
        NotifyObservers($"temperature changed {old}°C → {temp}°C");
    }

    public override string GetStatus() =>
        $"{Name}: {Temperature}°C {(IsOn ? "🌡️ ON" : "OFF")}";
}
