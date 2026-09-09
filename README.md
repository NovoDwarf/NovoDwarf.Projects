# NovoDwarf.Projects

NovoDwarf.Projects is a monorepo for experimental and applied projects: mathematics libraries, simulation tools, procedural graph tooling, game prototypes, home automation, embedded code, and shared utilities.

## Navigation

| Area | Description |
|---|---|
| [`Projects`](Projects/README.md) | Top-level project catalog. Start here when looking for a specific product or domain area. |
| [`Projects/Mathematics`](Projects/Mathematics/README.md) | Numerical methods, probability, graphs, optimization, graphics, processing, and a MAUI app. |
| [`Projects/Modeling`](Projects/Modeling/README.md) | Delta-time and event-driven simulation libraries with metric collection. |
| [`Projects/Procedural.NET`](Projects/Procedural.NET/README.md) | Procedural graph runtime, node packages, storage, and Avalonia designer. |
| [`Projects/Grekov`](Projects/Grekov/README.md) | Definition, package, localization, assembly, JSON, and XML loading framework. |
| [`Projects/SultanDynasty`](Projects/SultanDynasty/README.md) | Game/simulation prototype built around definitions, ECS components, generation, and Avalonia UI. |
| [`Projects/Home`](Projects/Home/README.md) | SmartHome, Home Assistant integrations, bot frontends, MAUI client, ESP32, and IR control. |
| [`Projects/Godot`](Projects/Godot/README.md) | Godot/Slang integration experiments. |
| [`Shared`](Shared/README.md) | Shared primitives, file-system abstractions, and utility services. |

## Repository Layout

| Path | Purpose |
|---|---|
| `NovoDwarf.slnx` | Main .NET solution for most C# projects. |
| `Projects/` | Product and domain projects. |
| `Shared/` | Libraries reused across multiple projects. |
| `Tests/` | Reserved test tree. No test `.csproj` files are currently present. |
| `LICENSE`, `LICENSE_RU` | License files. |

## Current Platform

- Most C# projects target `net11.0`.
- `NovoDwarf.Mathematics.SourceGenerators` targets `netstandard2.0`.
- MAUI projects target Android/iOS, and the mathematics app also targets MacCatalyst.
- The current local SDK is `11.0.100-preview.5.26302.115`.
- SmartHome also has its own solution at `Projects/Home/SmartHome/SmartHome.slnx`.

## Requirements

- .NET SDK 11 preview.
- Rider or another IDE/toolchain that understands `.slnx`.
- MAUI workloads and platform SDKs for MAUI applications.
- ESP-IDF or PlatformIO for `Projects/Home/JenueESP`.
- Home Assistant for `Projects/Home/CandyIR` and SmartHome integrations.

## Common Commands

Restore and build the main solution:

```powershell
dotnet restore NovoDwarf.slnx
dotnet build NovoDwarf.slnx
```

Restore and build SmartHome separately:

```powershell
dotnet restore Projects/Home/SmartHome/SmartHome.slnx
dotnet build Projects/Home/SmartHome/SmartHome.slnx
```

Run selected applications:

```powershell
dotnet run --project Projects/Procedural.NET/Procedural.NET.Designer/Procedural.NET.Designer.csproj
dotnet run --project Projects/SultanDynasty/SultanDynasty.Avalonia/SultanDynasty.Avalonia.csproj
dotnet run --project Projects/Home/SmartHome/Sources/SmartHome.Host/SmartHome.Host.csproj
```

## Known State

- This repository contains active prototypes and libraries; public APIs should be treated as unstable unless a project documents otherwise.
- Some Modeling projects reference `Shared\Modeling.Core\Modeling.Core.csproj`, which is not present in the current tree. A full solution build may require restoring that project or fixing the references.
- Several dependencies use preview or wildcard package versions, so restore results may depend on the currently available package feed state.

## License

See [`LICENSE`](LICENSE) and [`LICENSE_RU`](LICENSE_RU).
