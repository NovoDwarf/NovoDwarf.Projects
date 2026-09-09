# Shared

Shared contains small libraries reused by multiple NovoDwarf projects.

## Navigation

- [Repository root](../README.md)
- [Projects catalog](../Projects/README.md)
- [Mathematics](../Projects/Mathematics/README.md)
- [Modeling](../Projects/Modeling/README.md)
- [Procedural.NET](../Projects/Procedural.NET/README.md)
- [Grekov](../Projects/Grekov/README.md)
- [SultanDynasty](../Projects/SultanDynasty/README.md)

## Projects

| Project | Purpose |
|---|---|
| `NovoDwarf.Primitives` | Shared primitive types, colors, bitmaps, point sets, coordinates, fields, and storage structures. |
| `NovoDwarf.FS` | File-system and path abstractions with physical implementations. |
| `NovoDwarf.Utilities` | File services, collection helpers, string helpers, and argument exception extensions. |

## Build

From the repository root:

```powershell
dotnet build Shared/NovoDwarf.Primitives/NovoDwarf.Primitives.csproj
dotnet build Shared/NovoDwarf.FS/NovoDwarf.FS.csproj
dotnet build Shared/NovoDwarf.Utilities/NovoDwarf.Utilities.csproj
```
