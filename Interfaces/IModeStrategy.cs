namespace SmartHomeHub.Interfaces;

/// <summary>
/// Strategy-mönstret — definierar regler för ett driftläge (Eco, Normal, Party).
/// </summary>
public interface IModeStrategy
{
    string ModeName { get; }
    bool AllowCommand(ICommand command);
    int GetMaxTemperature();
    bool AllowBatchOperations();
}
