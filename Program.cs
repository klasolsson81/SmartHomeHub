using SmartHomeHub;
using SmartHomeHub.Services;
using SmartHomeHub.UI;

Logger.Instance.SuppressOutput(true);

var hub = new SmartHomeFacade();

hub.AddDevice(DeviceFactory.Create("lamp", "Living Room Lamp"));
hub.AddDevice(DeviceFactory.Create("thermostat", "Main Thermostat"));
hub.AddDevice(DeviceFactory.Create("doorlock", "Front Door"));

Logger.Instance.SuppressOutput(false);
Console.Clear();

var menu = new MenuHandler(hub);

while (menu.Run()) { }
