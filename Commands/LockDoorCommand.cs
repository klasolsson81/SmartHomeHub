using SmartHomeHub.Devices;
using SmartHomeHub.Interfaces;

namespace SmartHomeHub.Commands;

/// <summary>
/// Command — låser eller låser upp ett dörrlås. Undo gör motsatsen.
/// </summary>
public class LockDoorCommand : ICommand
{
    private readonly DoorLock _doorLock;
    private readonly bool _lockAction;

    public string Description => _lockAction ? $"Lock({_doorLock.Name})" : $"Unlock({_doorLock.Name})";

    public LockDoorCommand(DoorLock doorLock, bool lockAction = true)
    {
        _doorLock = doorLock;
        _lockAction = lockAction;
    }

    public void Execute()
    {
        if (_lockAction) _doorLock.Lock();
        else _doorLock.Unlock();
    }

    public void Undo()
    {
        if (_lockAction) _doorLock.Unlock();
        else _doorLock.Lock();
    }
}
