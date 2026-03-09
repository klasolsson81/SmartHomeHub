using SmartHomeHub;
using SmartHomeHub.Commands;
using SmartHomeHub.Devices;
using SmartHomeHub.Services;
using SmartHomeHub.Interfaces;
using SmartHomeHub.Strategies;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var hub = new SmartHomeFacade();

// Skapa enheter via Factory Method (Bonus)
var lamp = (Lamp)DeviceFactory.Create("lamp", "Living Room Lamp");
var thermostat = (Thermostat)DeviceFactory.Create("thermostat", "Main Thermostat");
var doorLock = (DoorLock)DeviceFactory.Create("doorlock", "Front Door");

hub.AddDevice(lamp);
hub.AddDevice(thermostat);
hub.AddDevice(doorLock);

// Singleton-verifiering
Console.Clear();
Console.WriteLine("  Singleton: same Logger instance? " +
    ReferenceEquals(Logger.Instance, Logger.Instance));

bool running = true;

while (running)
{
    PrintMenu();
    var input = Console.ReadLine()?.Trim();

    switch (input)
    {
        case "1":
            DeviceMenu(hub, lamp, thermostat, doorLock);
            break;
        case "2":
            TemperatureMenu(hub, thermostat);
            break;
        case "3":
            DoorMenu(hub, doorLock);
            break;
        case "4":
            ModeMenu(hub);
            break;
        case "5":
            hub.MorningRoutine();
            break;
        case "6":
            hub.GoodNightRoutine();
            break;
        case "7":
            BuildCustomRoutine(hub, lamp, thermostat, doorLock);
            break;
        case "8":
            hub.BatchToggleLamps(true);
            break;
        case "9":
            hub.UndoLast();
            break;
        case "10":
            ReplayMenu(hub);
            break;
        case "11":
            hub.ShowStatus();
            break;
        case "12":
            hub.ShowCommandHistory();
            break;
        case "13":
            ShowAuditTrail(hub);
            break;
        case "0":
            running = false;
            Console.WriteLine("\n  Bye! 👋");
            break;
        default:
            Console.WriteLine("  Invalid choice, try again.");
            break;
    }

    if (running)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\n  Press any key to continue...");
        Console.ResetColor();
        Console.ReadKey(true);
    }
}

// ── Menyer ──────────────────────────────────────────────────

static void PrintMenu()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
    Console.WriteLine("║         🏠 Smart Home Control Center          ║");
    Console.WriteLine("╠═══════════════════════════════════════════════╣");
    Console.WriteLine("║  1.  Toggle lamp (on/off)                     ║");
    Console.WriteLine("║  2.  Set temperature                          ║");
    Console.WriteLine("║  3.  Lock / Unlock door                       ║");
    Console.WriteLine("║  4.  Change mode (Eco/Normal/Party)           ║");
    Console.WriteLine("║  5.  Run Morning Routine                      ║");
    Console.WriteLine("║  6.  Run Good Night Routine                   ║");
    Console.WriteLine("║  7.  Build custom routine (Builder)           ║");
    Console.WriteLine("║  8.  Batch: all lamps ON                      ║");
    Console.WriteLine("║  9.  Undo last command                        ║");
    Console.WriteLine("║  10. Replay last commands                     ║");
    Console.WriteLine("║  11. Show status                              ║");
    Console.WriteLine("║  12. Show command history                     ║");
    Console.WriteLine("║  13. Show audit trail                         ║");
    Console.WriteLine("║  0.  Exit                                     ║");
    Console.WriteLine("╚═══════════════════════════════════════════════╝");
    Console.ResetColor();
    Console.Write("  Choose: ");
}

static void DeviceMenu(SmartHomeFacade hub, Lamp lamp, Thermostat thermostat, DoorLock doorLock)
{
    if (lamp.IsOn)
    {
        Console.WriteLine($"  {lamp.Name} is ON → turning OFF");
        hub.RunCommand(new TurnOffCommand(lamp));
    }
    else
    {
        Console.WriteLine($"  {lamp.Name} is OFF → turning ON");
        hub.RunCommand(new TurnOnCommand(lamp));
    }
}

static void TemperatureMenu(SmartHomeFacade hub, Thermostat thermostat)
{
    Console.Write($"  Current: {thermostat.Temperature}°C. New temperature: ");
    if (int.TryParse(Console.ReadLine(), out int temp))
        hub.RunCommand(new SetTemperatureCommand(thermostat, temp));
    else
        Console.WriteLine("  Invalid temperature.");
}

static void DoorMenu(SmartHomeFacade hub, DoorLock doorLock)
{
    if (doorLock.IsLocked)
    {
        Console.WriteLine($"  {doorLock.Name} is LOCKED → unlocking");
        hub.RunCommand(new LockDoorCommand(doorLock, false));
    }
    else
    {
        Console.WriteLine($"  {doorLock.Name} is UNLOCKED → locking");
        hub.RunCommand(new LockDoorCommand(doorLock, true));
    }
}

static void ModeMenu(SmartHomeFacade hub)
{
    Console.WriteLine("  1. Normal  2. Eco  3. Party");
    Console.Write("  Choose mode: ");
    var choice = Console.ReadLine()?.Trim();
    IModeStrategy mode = choice switch
    {
        "1" => new NormalMode(),
        "2" => new EcoMode(),
        "3" => new PartyMode(),
        _ => new NormalMode()
    };
    hub.SetMode(mode);
}

static void BuildCustomRoutine(SmartHomeFacade hub, Lamp lamp, Thermostat thermostat, DoorLock doorLock)
{
    Console.Write("  Routine name: ");
    var name = Console.ReadLine()?.Trim() ?? "Custom";

    var builder = new RoutineBuilder().SetName(name);
    bool building = true;

    while (building)
    {
        Console.WriteLine("  Add step:  1. Lamp ON  2. Lamp OFF  3. Set temp  4. Lock  5. Unlock  0. Done");
        Console.Write("  Step: ");
        var step = Console.ReadLine()?.Trim();

        switch (step)
        {
            case "1": builder.AddStep(new TurnOnCommand(lamp)); break;
            case "2": builder.AddStep(new TurnOffCommand(lamp)); break;
            case "3":
                Console.Write("  Temperature: ");
                if (int.TryParse(Console.ReadLine(), out int t))
                    builder.AddStep(new SetTemperatureCommand(thermostat, t));
                break;
            case "4": builder.AddStep(new LockDoorCommand(doorLock, true)); break;
            case "5": builder.AddStep(new LockDoorCommand(doorLock, false)); break;
            case "0": building = false; break;
        }
    }

    var routine = builder.Build();
    routine.Execute(hub);
}

static void ReplayMenu(SmartHomeFacade hub)
{
    Console.Write("  How many commands to replay? (default 5): ");
    var input = Console.ReadLine()?.Trim();
    int count = int.TryParse(input, out int n) ? n : 5;
    hub.ReplayLast(count);
}

static void ShowAuditTrail(SmartHomeFacade hub)
{
    Console.WriteLine("\n  Audit Trail:");
    foreach (var entry in hub.GetAuditTrail())
        Console.WriteLine($"    {entry}");

    if (hub.GetAuditTrail().Count == 0)
        Console.WriteLine("    (empty)");
}
