using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Strategies;

/// <summary>
/// Strategy — normalläge utan begränsningar. Max 30°C.
/// </summary>
public class NormalMode : IModeStrategy
{
    public string ModeName => "Normal";
    public bool AllowCommand(ICommand command) => true;
    public int GetMaxTemperature() => 30;
    public bool AllowBatchOperations() => true;
}
