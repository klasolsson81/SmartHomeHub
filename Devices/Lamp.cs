namespace SmartHomeHub.Devices;

/// <summary>
/// Enkel lampa — kan slås på och av.
/// </summary>
public class Lamp : DeviceBase
{
    public Lamp(string name = "Lamp") : base(name) { }

    public override string GetStatus() =>
        $"{Name}: {(IsOn ? "💡 ON" : "OFF")}";
}
