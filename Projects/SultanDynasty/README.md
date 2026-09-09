# SultanDynasty

SultanDynasty is a game/simulation prototype built around Grekov definitions, Friflo ECS components, character generation, simulation systems, and an Avalonia desktop UI.

## Navigation

- [Repository root](../../README.md)
- [Projects catalog](../README.md)
- [Grekov](../Grekov/README.md)
- [Shared libraries](../../Shared/README.md)

## Projects

| Project | Purpose |
|---|---|
| `SultanDynasty.Core` | Base domain enums and primitive domain types. |
| `SultanDynasty.Definitions` | Grekov definitions for characters, dynasties, ethnicity, religion, stats, goals, knowledge, memories, locations, languages, and titles. |
| `SultanDynasty.Components` | Friflo ECS components and containers for character state. |
| `SultanDynasty.Sim` | Simulation world, clock, systems, character generation pipeline, registries, and DI setup. |
| `SultanDynasty.Avalonia` | Desktop UI, data bootstrap, package catalog, and simulation host. |

## Run

From the repository root:

```powershell
dotnet run --project Projects/SultanDynasty/SultanDynasty.Avalonia/SultanDynasty.Avalonia.csproj
```

## Build

```powershell
dotnet build Projects/SultanDynasty/SultanDynasty.Sim/SultanDynasty.Sim.csproj
dotnet build Projects/SultanDynasty/SultanDynasty.Avalonia/SultanDynasty.Avalonia.csproj
```
