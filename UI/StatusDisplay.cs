using Spectre.Console;
using SmartHomeHub.Devices;
using SmartHomeHub.Interfaces;

namespace SmartHomeHub.UI;

/// <summary>
/// Renderar enhetsstatus och senaste notifikationer som en Spectre.Console-tabell.
/// </summary>
public static class StatusDisplay
{
    public static void Render(SmartHomeFacade hub)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold cyan]Smart Home Control Center[/]")
            .AddColumn("Device")
            .AddColumn("Status");

        table.AddRow("[yellow]Mode[/]", $"[yellow]{hub.CurrentModeName}[/]");

        foreach (var device in hub.Devices)
        {
            var (name, status) = device switch
            {
                Lamp l => (l.Name, l.IsOn ? "[green]ON[/]" : "[red]OFF[/]"),
                Thermostat t => (t.Name, $"{t.Temperature}°C {(t.IsOn ? "[green]ON[/]" : "[red]OFF[/]")}"),
                DoorLock d => (d.Name, d.IsLocked ? "[red]Locked[/]" : "[green]Unlocked[/]"),
                _ => (device.Name, device.IsOn ? "[green]ON[/]" : "[red]OFF[/]")
            };
            table.AddRow(Markup.Escape(name), status);
        }

        AnsiConsole.Write(table);

        var notifications = hub.GetNotifications();
        if (notifications.Count > 0)
        {
            var recent = notifications.TakeLast(3).ToList();
            AnsiConsole.MarkupLine("\n[grey]  Recent:[/]");
            foreach (var n in recent)
                AnsiConsole.MarkupLine($"[grey]    {Markup.Escape(n)}[/]");
        }
    }
}
