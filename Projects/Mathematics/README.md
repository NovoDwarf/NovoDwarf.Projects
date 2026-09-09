# NovoDwarf.Mathematics

Mathematics contains the repository's numerical and mathematical libraries plus a MAUI application that uses them.

## Navigation

- [Repository root](../../README.md)
- [Projects catalog](../README.md)
- [Shared libraries](../../Shared/README.md)

## Projects

| Project | Purpose |
|---|---|
| `NovoDwarf.Mathematics.Core` | Core abstractions, resources, utility methods, and shared mathematical types. |
| `NovoDwarf.Mathematics.Numerical` | Numerical functions, interpolation, transforms, and special functions. |
| `NovoDwarf.Mathematics.Probability` | Probability models and distributions built on core/numerical functionality. |
| `NovoDwarf.Mathematics.Graphs` | Graph structures and graph-related algorithms. |
| `NovoDwarf.Mathematics.Optimization` | Optimization algorithms and related helpers. |
| `NovoDwarf.Mathematics.Cryptography` | Cryptography-oriented helpers. |
| `NovoDwarf.Mathematics.Compression` | Compression algorithms. |
| `NovoDwarf.Mathematics.Ballistics` | Ballistics calculations. |
| `NovoDwarf.Mathematics.Graphics` | Graphics primitives and helpers. |
| `NovoDwarf.Mathematics.Processing` | Data processing utilities. |
| `NovoDwarf.Mathematics.MachineLearning` | Machine-learning module. |
| `NovoDwarf.Mathematics.System` | System-level mathematics module. |
| `NovoDwarf.Mathematics.DependencyInjection` | Dependency-injection registration for mathematics services. |
| `NovoDwarf.Mathematics.SourceGenerators` | Roslyn source generator targeting `netstandard2.0`. |
| `NovoDwarf.Mathematics.App.Maui` | MAUI app targeting Android, iOS, and MacCatalyst. |

## Build

From the repository root:

```powershell
dotnet build Projects/Mathematics/NovoDwarf.Mathematics.Core/NovoDwarf.Mathematics.Core.csproj
dotnet build Projects/Mathematics/NovoDwarf.Mathematics.App.Maui/NovoDwarf.Mathematics.App.Maui.csproj
```

MAUI builds require the matching workloads and platform SDKs.
