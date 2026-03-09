using SmartHomeHub.Commands;
using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Strategies;

public class EcoMode : IModeStrategy
{
    public string ModeName => "Eco 🌿";
    public int GetMaxTemperature() => 22;
    public bool AllowBatchOperations() => false;

    public bool AllowCommand(ICommand command)
    {
        if (command is SetTemperatureCommand)
            return true;

        if (command is TurnOnCommand)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠ EcoMode: {command.Description} blocked — save energy!");
            Console.ResetColor();
            return false;
        }

        return true;
    }
}
