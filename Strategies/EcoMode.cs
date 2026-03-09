using SmartHomeHub.Commands;
using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Strategies;

public class EcoMode : IModeStrategy
{
    public string ModeName => "Eco";
    public int GetMaxTemperature() => 22;
    public bool AllowBatchOperations() => false;

    public bool AllowCommand(ICommand command)
    {
        if (command is TurnOnCommand)
            return false;

        return true;
    }
}
