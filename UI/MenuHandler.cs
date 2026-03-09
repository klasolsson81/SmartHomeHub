using Spectre.Console;
using SmartHomeHub.Commands;
using SmartHomeHub.Devices;
using SmartHomeHub.Interfaces;
using SmartHomeHub.Services;
using SmartHomeHub.Strategies;

namespace SmartHomeHub.UI;

public class MenuHandler
{
    private readonly SmartHomeFacade _hub;

    public MenuHandler(SmartHomeFacade hub)
    {
        _hub = hub;
    }

    public bool Run()
    {
        try
        {
            AnsiConsole.Clear();
            StatusDisplay.Render(_hub);

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("\n[cyan]What do you want to do?[/]")
                    .PageSize(15)
                    .HighlightStyle("green")
                    .AddChoices(
                        "Toggle device",
                        "Set temperature",
                        "Lock / Unlock door",
                        "Change mode",
                        "Morning Routine",
                        "Good Night Routine",
                        "Build custom routine",
                        "Batch: all lamps ON",
                        "Add device",
                        "Undo last command",
                        "Replay last commands",
                        "Show command history",
                        "Show audit trail",
                        "Exit"));

            AnsiConsole.WriteLine();

            switch (choice)
            {
                case "Toggle device": ToggleDevice(); break;
                case "Set temperature": SetTemperature(); break;
                case "Lock / Unlock door": ToggleDoor(); break;
                case "Change mode": ChangeMode(); break;
                case "Morning Routine":
                    _hub.MorningRoutine();
                    ShowSuccess("Morning Routine complete");
                    break;
                case "Good Night Routine":
                    _hub.GoodNightRoutine();
                    ShowSuccess("Good Night Routine complete");
                    break;
                case "Build custom routine": BuildRoutine(); break;
                case "Batch: all lamps ON": HandleResult(_hub.BatchToggleLamps(true)); break;
                case "Add device": AddDevice(); break;
                case "Undo last command":
                    if (!_hub.UndoLast())
                        ShowWarning("Nothing to undo");
                    break;
                case "Replay last commands": ReplayCommands(); break;
                case "Show command history": ShowHistory(); break;
                case "Show audit trail": ShowAuditTrail(); break;
                case "Exit": return false;
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error: {Markup.Escape(ex.Message)}[/]");
        }

        AnsiConsole.Markup("\n[grey]Press Enter to continue...[/]");
        Console.ReadLine();
        return true;
    }

    private void ToggleDevice()
    {
        var devices = _hub.Devices.ToList();
        if (devices.Count == 0) { ShowWarning("No devices"); return; }

        var device = SelectDevice(devices, "Which device?");
        var result = device.IsOn
            ? _hub.RunCommand(new TurnOffCommand(device))
            : _hub.RunCommand(new TurnOnCommand(device));

        HandleResult(result);
    }

    private void SetTemperature()
    {
        var thermostats = _hub.Devices.OfType<Thermostat>().ToList();
        if (thermostats.Count == 0) { ShowWarning("No thermostats"); return; }

        var thermo = thermostats.Count == 1
            ? thermostats[0]
            : SelectDevice(thermostats, "Which thermostat?");

        var maxTemp = _hub.GetMaxTemperature();
        var temp = AnsiConsole.Prompt(
            new TextPrompt<int>($"  New temperature [grey](current {thermo.Temperature}°C, max {maxTemp}°C)[/]:")
                .Validate(t =>
                {
                    if (t < 5) return ValidationResult.Error("[red]Minimum 5°C[/]");
                    if (t > maxTemp) return ValidationResult.Error($"[red]Max {maxTemp}°C in {_hub.CurrentModeName} mode[/]");
                    return ValidationResult.Success();
                }));

        HandleResult(_hub.RunCommand(new SetTemperatureCommand(thermo, temp)));
    }

    private void ToggleDoor()
    {
        var locks = _hub.Devices.OfType<DoorLock>().ToList();
        if (locks.Count == 0) { ShowWarning("No door locks"); return; }

        var door = locks.Count == 1
            ? locks[0]
            : SelectDevice(locks, "Which door?");

        var result = door.IsLocked
            ? _hub.RunCommand(new LockDoorCommand(door, false))
            : _hub.RunCommand(new LockDoorCommand(door, true));

        HandleResult(result);
    }

    private void ChangeMode()
    {
        var modeChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]Select mode:[/]")
                .AddChoices("Normal", "Eco", "Party"));

        IModeStrategy mode = modeChoice switch
        {
            "Eco" => new EcoMode(),
            "Party" => new PartyMode(),
            _ => new NormalMode()
        };
        _hub.SetMode(mode);
        ShowSuccess($"Mode changed to {mode.ModeName}");
    }

    private void AddDevice()
    {
        var type = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]Device type:[/]")
                .AddChoices("Lamp", "Thermostat", "DoorLock"));

        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("  Device name:")
                .Validate(s => string.IsNullOrWhiteSpace(s)
                    ? ValidationResult.Error("[red]Name cannot be empty[/]")
                    : ValidationResult.Success()));

        _hub.AddDevice(DeviceFactory.Create(type, name));
        ShowSuccess($"Added {type}: {name}");
    }

    private void BuildRoutine()
    {
        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("  Routine name:")
                .Validate(s => string.IsNullOrWhiteSpace(s)
                    ? ValidationResult.Error("[red]Name cannot be empty[/]")
                    : ValidationResult.Success()));

        var builder = new RoutineBuilder().SetName(name);
        var devices = _hub.Devices.ToList();

        while (true)
        {
            var step = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[cyan]Add step:[/]")
                    .AddChoices("Turn ON device", "Turn OFF device", "Set temperature",
                        "Lock door", "Unlock door", "Done - run routine"));

            if (step == "Done - run routine") break;

            AddRoutineStep(builder, step, devices);
        }

        builder.Build().Execute(_hub);
        ShowSuccess($"Routine '{name}' executed");
    }

    private void AddRoutineStep(RoutineBuilder builder, string step, List<IDevice> devices)
    {
        switch (step)
        {
            case "Turn ON device":
                builder.AddStep(new TurnOnCommand(SelectDevice(devices, "Which device?")));
                break;
            case "Turn OFF device":
                builder.AddStep(new TurnOffCommand(SelectDevice(devices, "Which device?")));
                break;
            case "Set temperature":
                var thermos = devices.OfType<Thermostat>().ToList();
                if (thermos.Count == 0) { ShowWarning("No thermostats"); return; }
                var th = thermos.Count == 1 ? thermos[0] : SelectDevice(thermos, "Which thermostat?");
                var maxT = _hub.GetMaxTemperature();
                var t = AnsiConsole.Prompt(
                    new TextPrompt<int>($"  Temperature [grey](max {maxT}°C)[/]:")
                        .Validate(v =>
                        {
                            if (v < 5) return ValidationResult.Error("[red]Min 5°C[/]");
                            if (v > maxT) return ValidationResult.Error($"[red]Max {maxT}°C[/]");
                            return ValidationResult.Success();
                        }));
                builder.AddStep(new SetTemperatureCommand(th, t));
                break;
            case "Lock door":
            case "Unlock door":
                var doors = devices.OfType<DoorLock>().ToList();
                if (doors.Count == 0) { ShowWarning("No door locks"); return; }
                var door = doors.Count == 1 ? doors[0] : SelectDevice(doors, "Which door?");
                builder.AddStep(new LockDoorCommand(door, step == "Lock door"));
                break;
        }
    }

    private void ReplayCommands()
    {
        var count = AnsiConsole.Prompt(
            new TextPrompt<int>("  How many commands to replay?")
                .DefaultValue(5)
                .Validate(n => n switch
                {
                    < 1 => ValidationResult.Error("[red]Must be at least 1[/]"),
                    > 50 => ValidationResult.Error("[red]Max 50[/]"),
                    _ => ValidationResult.Success()
                }));

        var replayed = _hub.ReplayLast(count);
        ShowSuccess($"Replayed {replayed} command(s)");
    }

    private void ShowHistory()
    {
        var history = _hub.GetCommandHistory();
        if (history.Count == 0) { AnsiConsole.MarkupLine("[grey]  (empty)[/]"); return; }

        var table = new Table().Border(TableBorder.Simple).AddColumn("Command History");
        foreach (var entry in history)
            table.AddRow(Markup.Escape(entry));
        AnsiConsole.Write(table);
    }

    private void ShowAuditTrail()
    {
        var trail = _hub.GetAuditTrail();
        if (trail.Count == 0) { AnsiConsole.MarkupLine("[grey]  (empty)[/]"); return; }

        var table = new Table().Border(TableBorder.Simple).AddColumn("Audit Trail");
        foreach (var entry in trail)
            table.AddRow(Markup.Escape(entry));
        AnsiConsole.Write(table);
    }

    private static T SelectDevice<T>(List<T> devices, string title) where T : IDevice
    {
        var name = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[cyan]{title}[/]")
                .AddChoices(devices.Select(d => d.Name)));
        return devices.First(d => d.Name == name);
    }

    private static void HandleResult(CommandResult result)
    {
        if (!result.Success)
            ShowWarning(result.Message!);
    }

    private static void ShowSuccess(string msg) =>
        AnsiConsole.MarkupLine($"[green]  {Markup.Escape(msg)}[/]");

    private static void ShowWarning(string msg) =>
        AnsiConsole.MarkupLine($"[yellow]  {Markup.Escape(msg)}[/]");
}
