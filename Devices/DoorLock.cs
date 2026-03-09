namespace SmartHomeHub.Devices;

/// <summary>
/// Dörrlås — kan låsas och låsas upp, notifierar observers.
/// </summary>
public class DoorLock : DeviceBase
{
    public bool IsLocked { get; private set; } = true;

    public DoorLock(string name = "DoorLock") : base(name) { }

    public void Lock()
    {
        IsLocked = true;
        NotifyObservers("LOCKED 🔒");
    }

    public void Unlock()
    {
        IsLocked = false;
        NotifyObservers("UNLOCKED 🔓");
    }

    public override string GetStatus() =>
        $"{Name}: {(IsLocked ? "🔒 Locked" : "🔓 Unlocked")} {(IsOn ? "ON" : "OFF")}";
}
