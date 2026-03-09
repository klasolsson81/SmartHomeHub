namespace SmartHomeHub.Interfaces;

/// <summary>
/// Command-mönstret — kapslar in en åtgärd med Execute och Undo.
/// </summary>
public interface ICommand
{
    string Description { get; }
    void Execute();
    void Undo();
}
