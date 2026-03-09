namespace SmartHomeHub.Devices;

public class Lamp : DeviceBase
{
    public Lamp(string name = "Lamp") : base(name) { }

    public override string GetStatus() =>
        $"{Name}: {(IsOn ? "💡 ON" : "OFF")}";
}
