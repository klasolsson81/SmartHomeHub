![header](https://capsule-render.vercel.app/api?type=waving&color=0:1a1b27,50:24283b,100:414868&height=200&section=header&text=Smart%20Home%20Hub&fontSize=50&fontColor=7aa2f7&animation=fadeIn&fontAlignY=35&desc=OO%20Design%20%E2%80%94%20Designm%C3%B6nster%20i%20praktiken&descAlignY=55&descSize=18&descColor=a9b1d6)

<div align="center">

[![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](#)
[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)](#)
[![Spectre.Console](https://img.shields.io/badge/Spectre.Console-7aa2f7?style=for-the-badge&logo=windowsterminal&logoColor=white)](#)
[![Patterns](https://img.shields.io/badge/7_Design_Patterns-bb9af7?style=for-the-badge&logo=dotnet&logoColor=white)](#)

<br/>

En interaktiv konsollapplikation som simulerar ett smart hem-system.<br/>
Byggt med **7 designmönster** och **Spectre.Console** for modernt terminalbaserat UI med pilnavigering och validering.

</div>

<br/>

## &nbsp;Kör programmet

```bash
cd SmartHomeHub
dotnet run
```

> [!NOTE]
> Kräver .NET 10 SDK. Appen startar med 3 enheter: Lamp, Termostat och Dörrlås.

<br/>

## &nbsp;Projektstruktur

```
SmartHomeHub/
├── Interfaces/          # Kontrakt (IDevice, ICommand, IObserver, IModeStrategy)
├── Devices/             # Lamp, Thermostat, DoorLock + gemensam DeviceBase
├── Commands/            # TurnOnCommand, TurnOffCommand, SetTemperatureCommand, LockDoorCommand
├── Observers/           # DashboardObserver, LoggerObserver, AuditObserver
├── Strategies/          # EcoMode, NormalMode, PartyMode
├── Services/            # Logger (Singleton), CommandInvoker, DeviceFactory, RoutineBuilder
├── UI/                  # MenuHandler (interaktiv meny), StatusDisplay (enhetsstatus)
├── SmartHomeFacade.cs   # Facade — huvudingång till hela systemet
└── Program.cs           # Startar appen, minimal — delegerar till MenuHandler
```

<br/>

## &nbsp;Designmönster

<table>
<tr>
<td width="50%" valign="top">

### 1. Observer
![Observer](https://img.shields.io/badge/Behavioral-Observer-7aa2f7?style=flat-square)

**Problem:** När en enhet ändrar state behöver flera delar av systemet veta — dashboard, logg, audit — utan att enheten ska känna till dem.

**Lösning:** `DeviceBase` håller en lista av `IObserver`. Vid state-ändring anropas `NotifyObservers()` som meddelar alla prenumeranter. Tre observers med olika ansvar: `DashboardObserver`, `LoggerObserver`, `AuditObserver`.

**Filer:** `Observers/`, `Devices/DeviceBase.cs`

</td>
<td width="50%" valign="top">

### 2. Command
![Command](https://img.shields.io/badge/Behavioral-Command-7aa2f7?style=flat-square)

**Problem:** Vi vill kunna köa, logga, ångra och återspela åtgärder — inte bara anropa metoder direkt.

**Lösning:** Varje åtgärd är ett `ICommand`-objekt med `Execute()` och `Undo()`. `CommandInvoker` hanterar köning, historik, undo och replay av senaste N kommandon.

**Filer:** `Commands/`, `Services/CommandInvoker.cs`

</td>
</tr>
<tr>
<td width="50%" valign="top">

### 3. Strategy
![Strategy](https://img.shields.io/badge/Behavioral-Strategy-7aa2f7?style=flat-square)

**Problem:** Systemet ska bete sig olika beroende på läge (Eco, Normal, Party) utan en massa if-satser.

**Lösning:** `IModeStrategy` definierar regler: tillåtna kommandon, max temperatur, batch-operationer. Facade frågar aktiv strategy innan varje kommando — strategin påverkar kommandon, temperatur och batch.

**Filer:** `Strategies/`, `Interfaces/IModeStrategy.cs`

</td>
<td width="50%" valign="top">

### 4. Facade
![Facade](https://img.shields.io/badge/Structural-Facade-bb9af7?style=flat-square)

**Problem:** UI-lagret ska inte behöva veta om observers, invokers, strategies och deras samspel.

**Lösning:** `SmartHomeFacade` exponerar ett rent API: `RunCommand()`, `SetMode()`, `MorningRoutine()`, `AddDevice()` m.m. Returnerar `CommandResult` med status och felmeddelande.

**Filer:** `SmartHomeFacade.cs`

</td>
</tr>
<tr>
<td width="50%" valign="top">

### 5. Singleton
![Singleton](https://img.shields.io/badge/Creational-Singleton-e0af68?style=flat-square)

**Problem:** Loggning ska ske konsekvent överallt, med en enda delad instans.

**Lösning:** `Logger` använder `Lazy<T>` för thread-safe singleton. Används av `LoggerObserver`, `CommandInvoker` och `SmartHomeFacade` — alla delar samma instans.

**Filer:** `Services/Logger.cs`

</td>
<td width="50%" valign="top">

### 6. Factory Method &nbsp;![Bonus](https://img.shields.io/badge/BONUS-00C853?style=flat-square)
![Factory](https://img.shields.io/badge/Creational-Factory-e0af68?style=flat-square)

**Problem:** Vi vill skapa enheter baserat på en sträng utan att anroparen behöver veta vilken konkret klass som skapas.

**Lösning:** `DeviceFactory.Create("lamp", "Kitchen Lamp")` returnerar rätt `IDevice`. Lätt att utöka med nya enhetstyper.

**Filer:** `Services/DeviceFactory.cs`

</td>
</tr>
<tr>
<td colspan="2" align="center" valign="top">

### 7. Builder &nbsp;![Bonus](https://img.shields.io/badge/BONUS-00C853?style=flat-square)
![Builder](https://img.shields.io/badge/Creational-Builder-e0af68?style=flat-square)

**Problem:** Rutiner (sekvenser av kommandon) kan vara komplexa att bygga, och vi vill ha en tydlig, stegvis konstruktion.

**Lösning:** `RoutineBuilder` med fluent API: `.SetName("Movie Night").AddStep(cmd1).AddStep(cmd2).Build()` returnerar en `Routine` som exekveras via Facade.

**Filer:** `Services/RoutineBuilder.cs`

</td>
</tr>
</table>

<br/>

## &nbsp;Klassdiagram

```mermaid
classDiagram
    direction TB

    class IDevice {
        <<interface>>
        +string Name
        +bool IsOn
        +TurnOn()
        +TurnOff()
        +GetStatus() string
    }

    class ICommand {
        <<interface>>
        +string Description
        +Execute()
        +Undo()
    }

    class IObserver {
        <<interface>>
        +Update(deviceName, eventDescription)
    }

    class IModeStrategy {
        <<interface>>
        +string ModeName
        +AllowCommand(ICommand) bool
        +GetMaxTemperature() int
        +AllowBatchOperations() bool
    }

    class IObservable {
        <<interface>>
        +Subscribe(IObserver)
        +Unsubscribe(IObserver)
    }

    class DeviceBase {
        <<abstract>>
        +string Name
        +bool IsOn
        #NotifyObservers(string)
        +TurnOn()
        +TurnOff()
        +GetStatus()* string
    }

    class Lamp {
        +GetStatus() string
    }

    class Thermostat {
        +int Temperature
        +SetTemperature(int)
        +GetStatus() string
    }

    class DoorLock {
        +bool IsLocked
        +Lock()
        +Unlock()
        +GetStatus() string
    }

    class TurnOnCommand {
        +Execute()
        +Undo()
    }

    class TurnOffCommand {
        +Execute()
        +Undo()
    }

    class SetTemperatureCommand {
        +int NewTemp
        +Execute()
        +Undo()
    }

    class LockDoorCommand {
        +Execute()
        +Undo()
    }

    class CommandInvoker {
        +Enqueue(ICommand)
        +ExecuteAll()
        +ExecuteSingle(ICommand)
        +UndoLast() bool
        +GetLastCommands(int) IReadOnlyList
        +GetHistory() IReadOnlyList
    }

    class SmartHomeFacade {
        +IReadOnlyList~IDevice~ Devices
        +string CurrentModeName
        +AddDevice(IDevice)
        +RunCommand(ICommand) CommandResult
        +SetMode(IModeStrategy)
        +MorningRoutine()
        +GoodNightRoutine()
        +BatchToggleLamps(bool) CommandResult
        +UndoLast() bool
        +ReplayLast(int) int
    }

    class CommandResult {
        <<record>>
        +bool Success
        +string? Message
        +Ok()$ CommandResult
        +Blocked(string)$ CommandResult
    }

    class Logger {
        <<Singleton>>
        +Logger Instance$
        +Log(string)
        +SuppressOutput(bool)
    }

    class DeviceFactory {
        +Create(type, name)$ IDevice
    }

    class RoutineBuilder {
        +SetName(string) RoutineBuilder
        +AddStep(ICommand) RoutineBuilder
        +Build() Routine
    }

    class Routine {
        +string Name
        +Execute(Action~ICommand~)
    }

    class NormalMode {
        +ModeName = "Normal"
        +GetMaxTemperature() 30
    }

    class EcoMode {
        +ModeName = "Eco"
        +GetMaxTemperature() 22
    }

    class PartyMode {
        +ModeName = "Party"
        +GetMaxTemperature() 35
    }

    class DashboardObserver {
        +IReadOnlyList Notifications
        +Update(string, string)
    }

    class LoggerObserver {
        +Update(string, string)
    }

    class AuditObserver {
        +IReadOnlyList AuditTrail
        +Update(string, string)
    }

    class MenuHandler {
        +Run() bool
    }

    IDevice <|.. DeviceBase
    IObservable <|.. DeviceBase
    DeviceBase <|-- Lamp
    DeviceBase <|-- Thermostat
    DeviceBase <|-- DoorLock

    ICommand <|.. TurnOnCommand
    ICommand <|.. TurnOffCommand
    ICommand <|.. SetTemperatureCommand
    ICommand <|.. LockDoorCommand

    IModeStrategy <|.. NormalMode
    IModeStrategy <|.. EcoMode
    IModeStrategy <|.. PartyMode

    IObserver <|.. DashboardObserver
    IObserver <|.. LoggerObserver
    IObserver <|.. AuditObserver

    DeviceBase o-- IObserver : observers

    SmartHomeFacade *-- CommandInvoker
    SmartHomeFacade *-- DashboardObserver
    SmartHomeFacade *-- LoggerObserver
    SmartHomeFacade *-- AuditObserver
    SmartHomeFacade o-- IModeStrategy : active mode
    SmartHomeFacade o-- IDevice : devices

    CommandInvoker o-- ICommand : history

    RoutineBuilder o-- ICommand : steps
    RoutineBuilder --> Routine : builds

    DeviceFactory ..> IDevice : creates

    MenuHandler --> SmartHomeFacade : uses
    LoggerObserver --> Logger : uses
```

<br/>

## &nbsp;Demo

<div align="center">
<img src="UI/demo.png" alt="Smart Home Hub — huvudmeny" width="500" />
</div>

> [!TIP]
> Menyn navigeras med piltangenter. Temperatur och andra input valideras i realtid — felaktiga värden blockeras direkt.

<br/>

## &nbsp;Clean Code

<table>
<tr>
<td width="25%" align="center"><strong>SRP</strong></td>
<td>UI-logik i <code>UI/</code>, affärslogik i Facade/Commands/Services. Ingen Console-output i affärslagret.</td>
</tr>
<tr>
<td align="center"><strong>DRY</strong></td>
<td>Gemensam <code>DeviceBase</code> för observer-hantering, <code>HandleResult()</code> för enhetlig felvisning.</td>
</tr>
<tr>
<td align="center"><strong>Felhantering</strong></td>
<td>try-catch i menyn, <code>CommandResult</code> för kontrollerade felfall, Spectre.Console-validering på alla input.</td>
</tr>
<tr>
<td align="center"><strong>Lager</strong></td>
<td><code>Program → MenuHandler → SmartHomeFacade → Commands/Invoker/Devices</code></td>
</tr>
</table>

<br/>

## &nbsp;Reflektion — När man INTE ska använda mönster

Designmönster är verktyg, inte mål i sig. **Observer** passar vid en-till-många-relationer, men om bara en klass bryr sig om en ändring är ett enkelt metodanrop bättre — mönstret skapar onödig komplexitet. **Singleton** är bekvämt för loggning, men kan göra testning svår och skapa dolda beroenden — i ett större projekt hade jag använt dependency injection istället. **Command** är perfekt när man behöver undo/replay, men för en enkel "byt lampan" utan historik är det overkill.

> [!IMPORTANT]
> Nyckeln är balans: **enklaste lösningen som löser problemet**. Mönster är riktlinjer, inte magi — de ska förenkla, inte imponera.

<br/>

## &nbsp;Författare

<div align="center">

<a href="https://github.com/klasolsson81">
<img src="UI/profile-circle.svg" width="160" alt="Klas Olsson" />
</a>

### Klas Olsson

[![Typing SVG](https://readme-typing-svg.demolab.com/?lines=Full-Stack+Developer+%7C+.NET+%7C+React+%7C+AI;NBI-Handelsakademin+%E2%80%94+Systemutvecklare+.NET&font=JetBrains+Mono&center=true&width=500&height=45&color=a9b1d6&vCenter=true&pause=1000&size=16&duration=3000)](https://klasolsson.se)

<br/>

[![Portfolio](https://img.shields.io/badge/klasolsson.se-7aa2f7?style=for-the-badge&logo=googlechrome&logoColor=white)](https://klasolsson.se)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://linkedin.com/in/klasolsson81)
[![Email](https://img.shields.io/badge/Email-bb9af7?style=for-the-badge&logo=gmail&logoColor=white)](mailto:klasolsson81@gmail.com)
[![GitHub](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/klasolsson81)

</div>

![footer](https://capsule-render.vercel.app/api?type=waving&color=0:1a1b27,50:24283b,100:414868&height=120&section=footer)
