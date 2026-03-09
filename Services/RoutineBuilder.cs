using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Services;

/// <summary>
/// Builder — bygger rutiner (sekvenser av kommandon) stegvis.
/// Gör det tydligt och läsbart att sätta ihop komplexa sekvenser.
/// </summary>
public class RoutineBuilder
{
    private readonly List<ICommand> _steps = [];
    private string _name = "Unnamed Routine";

    public RoutineBuilder SetName(string name)
    {
        _name = name;
        return this;
    }

    public RoutineBuilder AddStep(ICommand command)
    {
        _steps.Add(command);
        return this;
    }

    public Routine Build()
    {
        if (_steps.Count == 0)
            throw new InvalidOperationException("Routine must have at least one step.");

        return new Routine(_name, [.. _steps]);
    }
}

public class Routine
{
    public string Name { get; }
    private readonly List<ICommand> _steps;

    public Routine(string name, List<ICommand> steps)
    {
        Name = name;
        _steps = steps;
    }

    public void Execute(Action<ICommand> runCommand)
    {
        foreach (var step in _steps)
            runCommand(step);
    }
}
