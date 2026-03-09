namespace SmartHomeHub.Interfaces;

/// <summary>
/// Observer-mönstret — prenumerant som reagerar på enhetsändringar.
/// </summary>
public interface IObserver
{
    void Update(string deviceName, string eventDescription);
}
