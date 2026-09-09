# Procedural.NET

Procedural.NET contains a procedural graph runtime, storage layer, node packages, and an Avalonia desktop designer.

## Navigation

- [Repository root](../../README.md)
- [Projects catalog](../README.md)
- [Shared libraries](../../Shared/README.md)
- [Mathematics](../Mathematics/README.md)

## Projects

| Project | Purpose |
|---|---|
| `Procedural.NET.Core` | Core graph document, nodes, ports, commands, runtime contracts, caching, rules, and selection model. |
| `Procedural.NET` | Main runtime/facade project. |
| `Procedural.NET.Storage` | Graph sessions, serializers, repositories, runtime store, macro definitions, and presets. |
| `Procedural.NET.Nodes` | Base node package. |
| `Procedural.NET.Nodes.Spatial` | Spatial nodes. |
| `Procedural.NET.Nodes.Terrain` | Terrain nodes, erosion, selectors, display/export helpers, and terrain materials. |
| `Procedural.NET.Designer` | Avalonia desktop designer with Silk.NET/OpenGL rendering. |

## Run

From the repository root:

```powershell
dotnet run --project Projects/Procedural.NET/Procedural.NET.Designer/Procedural.NET.Designer.csproj
```

## Build

```powershell
dotnet build Projects/Procedural.NET/Procedural.NET.Core/Procedural.NET.Core.csproj
dotnet build Projects/Procedural.NET/Procedural.NET.Designer/Procedural.NET.Designer.csproj
```
