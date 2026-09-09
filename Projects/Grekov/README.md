# Grekov

Grekov is a definition and package loading framework used by game/domain projects in this repository.

## Navigation

- [Repository root](../../README.md)
- [Projects catalog](../README.md)
- [SultanDynasty](../SultanDynasty/README.md)
- [Shared libraries](../../Shared/README.md)

## Projects

| Project | Purpose |
|---|---|
| `Grekov.Core` | Core contracts and models. |
| `Grekov.Packaging` | Package metadata, load order, and path/package resolution. |
| `Grekov.Definitions` | Definition discovery, scanning, and loading. |
| `Grekov.Localizations` | Localization support for definition packages. |
| `Grekov.Assemblies` | Loading definitions from assemblies. |
| `Grekov` | Main facade and dependency-injection integration. |
| `Readers/Grekov.Readers.Json` | JSON definition reader. |
| `Readers/Grekov.Readers.Xml` | XML definition reader. |
| `Examples/Grekov.Example` | Console usage example. |

## Build And Run

From the repository root:

```powershell
dotnet build Projects/Grekov/Grekov/Grekov.csproj
dotnet run --project Projects/Grekov/Examples/Grekov.Example/Grekov.Example.csproj
```
