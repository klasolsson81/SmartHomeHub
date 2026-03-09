namespace SmartHomeHub.Interfaces;

public interface ICommand
{
    string Description { get; }
    void Execute();
    void Undo();
}
