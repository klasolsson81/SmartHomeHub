using SmartHomeHub;
using SmartHomeHub.Commands;
using SmartHomeHub.Devices;
using SmartHomeHub.Services;
using SmartHomeHub.Strategies;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ════════════════════════════════════════════════════════════
//  Smart Home Hub — Designmönster i praktiken
// ════════════════════════════════════════════════════════════

Console.WriteLine("╔═══════════════════════════════════════════════╗");
Console.WriteLine("║      🏠 Smart Home Control Center 🏠         ║");
Console.WriteLine("╚═══════════════════════════════════════════════╝\n");

// --- Singleton: samma Logger-instans överallt ---
var logger1 = Logger.Instance;
var logger2 = Logger.Instance;
Console.WriteLine($"  Singleton check: same Logger? {ReferenceEquals(logger1, logger2)}\n");

// --- Factory Method (Bonus): skapa enheter via factory ---
var hub = new SmartHomeFacade();

var lamp = (Lamp)DeviceFactory.Create("lamp", "Living Room Lamp");
var thermostat = (Thermostat)DeviceFactory.Create("thermostat", "Main Thermostat");
var doorLock = (DoorLock)DeviceFactory.Create("doorlock", "Front Door");

hub.AddDevice(lamp);
hub.AddDevice(thermostat);
hub.AddDevice(doorLock);

hub.ShowStatus();

// --- Observer: 3 lyssnare (Dashboard + Logger + Audit) reagerar ---
Console.WriteLine("\n── Observer Demo ──────────────────────────────");
hub.RunCommand(new TurnOnCommand(lamp));

// --- Command: köa och köra kommandon ---
Console.WriteLine("\n── Command Demo (Queue) ──────────────────────");
hub.QueueCommand(new TurnOnCommand(thermostat));
hub.QueueCommand(new SetTemperatureCommand(thermostat, 24));
hub.QueueCommand(new LockDoorCommand(doorLock, false));
hub.ExecuteQueue();

hub.ShowStatus();

// --- Command: Undo ---
Console.WriteLine("\n── Command Undo Demo ─────────────────────────");
hub.UndoLast();
hub.ShowStatus();

// --- Command: Replay senaste 3 ---
Console.WriteLine("\n── Command Replay Demo ───────────────────────");
hub.ReplayLast(3);

// --- Strategy: byt mode och se hur beteendet ändras ---
Console.WriteLine("\n── Strategy Demo ─────────────────────────────");

hub.SetMode(new EcoMode());
Console.WriteLine("  Trying TurnOn in EcoMode:");
hub.RunCommand(new TurnOnCommand(lamp));
Console.WriteLine("  Trying batch in EcoMode:");
hub.BatchToggleLamps(true);

hub.SetMode(new PartyMode());
Console.WriteLine("  Batch in PartyMode:");
hub.BatchToggleLamps(true);
hub.RunCommand(new SetTemperatureCommand(thermostat, 28));

hub.ShowStatus();

// --- Facade: MorningRoutine & GoodNightRoutine ---
Console.WriteLine("\n── Facade Routines Demo ──────────────────────");
hub.SetMode(new NormalMode());
hub.MorningRoutine();
hub.ShowStatus();

hub.GoodNightRoutine();
hub.ShowStatus();

// --- Builder (Bonus): bygg custom rutin stegvis ---
Console.WriteLine("\n── Builder Demo (Custom Routine) ─────────────");
var customRoutine = new RoutineBuilder()
    .SetName("Movie Night")
    .AddStep(new TurnOffCommand(lamp))
    .AddStep(new SetTemperatureCommand(thermostat, 23))
    .AddStep(new LockDoorCommand(doorLock, true))
    .Build();

customRoutine.Execute(hub);
hub.ShowStatus();

// --- Historik & Audit ---
Console.WriteLine("\n── Command History ───────────────────────────");
hub.ShowCommandHistory();

Console.WriteLine("\n── Audit Trail (last 5) ──────────────────────");
foreach (var entry in hub.GetAuditTrail().TakeLast(5))
    Console.WriteLine($"    {entry}");

Console.WriteLine("\n╔═══════════════════════════════════════════════╗");
Console.WriteLine("║           ✅ Demo Complete!                   ║");
Console.WriteLine("╚═══════════════════════════════════════════════╝");
