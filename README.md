# Smart Home Control Center

En konsollapplikation som simulerar ett smart hem-system. Byggt med 7 designmönster för att visa hur de löser verkliga problem — inte som pynt, utan som verktyg.

## Kör programmet

```bash
cd SmartHomeHub
dotnet run
```

Kräver .NET 10 SDK.

## Projektstruktur

```
SmartHomeHub/
├── Interfaces/          # Kontrakt (IDevice, ICommand, IObserver, IModeStrategy)
├── Devices/             # Lamp, Thermostat, DoorLock + gemensam DeviceBase
├── Commands/            # TurnOnCommand, TurnOffCommand, SetTemperatureCommand, LockDoorCommand
├── Observers/           # DashboardObserver, LoggerObserver, AuditObserver
├── Strategies/          # EcoMode, NormalMode, PartyMode
├── Services/            # Logger (Singleton), CommandInvoker, DeviceFactory, RoutineBuilder
├── SmartHomeFacade.cs   # Facade — huvudingång till hela systemet
└── Program.cs           # Interaktiv meny — styr hemmet via Facade
```

## Designmönster

### 1. Observer — Live-uppdateringar (`Observers/`)
**Problem:** När en enhet ändrar state behöver flera delar av systemet veta om det — dashboard, logg, audit — utan att enheten ska behöva känna till dem.
**Lösning:** `DeviceBase` håller en lista av `IObserver`. När state ändras (TurnOn, SetTemperature, Lock) anropas `NotifyObservers()` som meddelar alla prenumeranter. Tre observers — `DashboardObserver`, `LoggerObserver`, `AuditObserver` — reagerar med olika ansvar.

### 2. Command — Kommandon som objekt (`Commands/`, `Services/CommandInvoker.cs`)
**Problem:** Vi vill kunna köa, logga, ångra och återspela åtgärder, inte bara "anropa metoder direkt".
**Lösning:** Varje åtgärd är ett `ICommand`-objekt med `Execute()` och `Undo()`. `CommandInvoker` hanterar köning, historik, undo och replay av senaste N kommandon. Allt loggas automatiskt via Singleton-loggern.

### 3. Strategy — Lägesbaserat beteende (`Strategies/`)
**Problem:** Systemet ska bete sig olika beroende på läge (Eco, Normal, Party) utan en massa if-satser.
**Lösning:** `IModeStrategy` definierar regler: vilka kommandon tillåts, max temperatur, batch-operationer. `EcoMode` blockerar TurnOn och batch, `PartyMode` tillåter allt med högre temp. Facade frågar aktiv strategy innan varje kommando körs — strategin påverkar kommandon, temperatur och batch.

### 4. Facade — Rent API (`SmartHomeFacade.cs`)
**Problem:** `Program.cs` ska inte behöva veta om observers, invokers, strategies och deras samspel.
**Lösning:** `SmartHomeFacade` exponerar `AddDevice()`, `RunCommand()`, `SetMode()`, `MorningRoutine()`, `GoodNightRoutine()`, `BatchToggleLamps()` m.m. Allt styrs via Facade — main-programmet petar aldrig i interna detaljer.

### 5. Singleton — En gemensam Logger (`Services/Logger.cs`)
**Problem:** Loggning ska ske konsekvent överallt, och alla delar ska dela samma instans.
**Lösning:** `Logger` använder `Lazy<T>` för thread-safe singleton. Används av `LoggerObserver`, `CommandInvoker`, `SmartHomeFacade` — alla samma instans, verifierat i demo med `ReferenceEquals`.

### 6. Factory Method (Bonus) — Skapa enheter (`Services/DeviceFactory.cs`)
**Problem:** Vi vill kunna skapa enheter baserat på en sträng (t.ex. från input) utan att anroparen behöver veta vilken konkret klass som skapas.
**Lösning:** `DeviceFactory.Create("lamp", "Kitchen Lamp")` returnerar rätt `IDevice`-implementering. Lätt att utöka med nya enhetstyper.

### 7. Builder (Bonus) — Bygga rutiner stegvis (`Services/RoutineBuilder.cs`)
**Problem:** Rutiner (sekvenser av kommandon) kan vara komplexa att bygga, och vi vill ha en tydlig, stegvis konstruktion.
**Lösning:** `RoutineBuilder` med fluent API: `.SetName("Movie Night").AddStep(cmd1).AddStep(cmd2).Build()` returnerar en `Routine` som kan exekveras via Facade.

## Demo Output (utdrag)

```
╔═══════════════════════════════════════════════╗
║         🏠 Smart Home Control Center          ║
╠═══════════════════════════════════════════════╣
║  1.  Toggle lamp (on/off)                     ║
║  2.  Set temperature                          ║
║  3.  Lock / Unlock door                       ║
║  4.  Change mode (Eco/Normal/Party)           ║
║  5.  Run Morning Routine                      ║
║  6.  Run Good Night Routine                   ║
║  7.  Build custom routine (Builder)           ║
║  8.  Batch: all lamps ON                      ║
║  9.  Undo last command                        ║
║  10. Replay last commands                     ║
║  11. Show status                              ║
║  12. Show command history                     ║
║  13. Show audit trail                         ║
║  0.  Exit                                     ║
╚═══════════════════════════════════════════════╝
  Choose: 1
  Living Room Lamp is OFF → turning ON
  [DASHBOARD] Living Room Lamp → turned ON
  [LOG] Living Room Lamp → turned ON
  [AUDIT] Living Room Lamp: turned ON

  Choose: 4
  1. Normal  2. Eco  3. Party
  Choose mode: 2
  ★ Mode changed to: Eco 🌿

  Choose: 1
  ⚠ EcoMode: TurnOn(Living Room Lamp) blocked — save energy!
```

## Reflektion — När man INTE ska använda mönster

Designmönster är verktyg, inte mål i sig. Observer passar när det finns en-till-många-relationer, men om bara en klass bryr sig om en ändring är en enkel metodanrop bättre — mönstret skapar onödig komplexitet. Singleton är bekvämt för loggning, men kan göra testning svår och skapa dolda beroenden — i ett större projekt hade jag använt dependency injection istället. Command är perfekt när man behöver undo/replay, men för en enkel "byt lampan" utan historik är det overkill.

Nyckeln är balans: **enklaste lösningen som löser problemet**. Mönster är riktlinjer, inte magi — de ska förenkla, inte imponera.
