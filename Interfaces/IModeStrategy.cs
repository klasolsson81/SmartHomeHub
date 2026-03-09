namespace SmartHomeHub.Interfaces;

public interface IModeStrategy
{
    string ModeName { get; }
    bool AllowCommand(ICommand command);
    int GetMaxTemperature();
    bool AllowBatchOperations();
}
