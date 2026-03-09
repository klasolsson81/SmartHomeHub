namespace SmartHomeHub.Interfaces;

/// <summary>
/// Kontrakt för alla smarta enheter i systemet.
/// </summary>
public interface IDevice
{
    string Name { get; }
    bool IsOn { get; }
    void TurnOn();
    void TurnOff();
    string GetStatus();
}
