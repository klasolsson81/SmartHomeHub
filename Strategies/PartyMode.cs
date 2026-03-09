using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Strategies;

/// <summary>
/// Strategy — partyläge utan begränsningar. Max 35°C.
/// </summary>
public class PartyMode : IModeStrategy
{
    public string ModeName => "Party 🎉";
    public bool AllowCommand(ICommand command) => true;
    public int GetMaxTemperature() => 35;
    public bool AllowBatchOperations() => true;
}
