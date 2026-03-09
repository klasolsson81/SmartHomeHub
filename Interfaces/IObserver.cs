namespace SmartHomeHub.Interfaces;

public interface IObserver
{
    void Update(string deviceName, string eventDescription);
}
