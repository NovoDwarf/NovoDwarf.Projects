# NovoDwarf.Modeling

Modeling contains simulation libraries and examples for time-stepped and event-driven systems.

## Navigation

- [Repository root](../../README.md)
- [Projects catalog](../README.md)
- [Mathematics](../Mathematics/README.md)
- [Shared libraries](../../Shared/README.md)

## Projects

| Project | Purpose |
|---|---|
| `NovoDwarf.Modeling.Core` | Base simulation entities, node options, requests, ranges, and statistics sinks. |
| `NovoDwarf.Modeling.Logging` | Metric events, collectors, sinks, reports, and in-memory metric storage. |
| `NovoDwarf.Modeling.DeltaT` | Delta-time simulation components. |
| `NovoDwarf.Modeling.EventDriven` | Event-driven simulation components with source, queue, service, and sink nodes. |
| `NovoDwarf.Modeling.DeltaT.Example` | Console example for DeltaT simulation. |
| `NovoDwarf.Modeling.EventDriven.Example` | Console example for event-driven simulation. |

## Build And Run

From the repository root:

```powershell
dotnet build Projects/Modeling/NovoDwarf.Modeling.Core/NovoDwarf.Modeling.Core.csproj
dotnet run --project Projects/Modeling/NovoDwarf.Modeling.DeltaT.Example/NovoDwarf.Modeling.DeltaT.Example.csproj
dotnet run --project Projects/Modeling/NovoDwarf.Modeling.EventDriven.Example/NovoDwarf.Modeling.EventDriven.Example.csproj
```

## Known State

`NovoDwarf.Modeling.DeltaT`, `NovoDwarf.Modeling.EventDriven`, and their examples currently reference `Shared\Modeling.Core\Modeling.Core.csproj`, which is not present in the repository tree. Restore that project or fix those references before relying on a full Modeling build.
