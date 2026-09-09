# SmartHome

SmartHome is a .NET solution for controlling Home Assistant devices through a host application, bot integrations, shared services, and a MAUI client.

## Navigation

- [Repository root](../../../README.md)
- [Projects catalog](../../README.md)
- [Home](../README.md)

## Solution

The solution file is [`SmartHome.slnx`](SmartHome.slnx).

## Projects

| Project | Purpose |
|---|---|
| `Sources/SmartHome.Core` | Common bot actions, payload codec, and domain model. |
| `Sources/SmartHome.HomeAssistant` | HTTP client and services for Home Assistant. |
| `Sources/SmartHome.Telegram` | Telegram bot integration. |
| `Sources/SmartHome.VK` | VK bot integration. |
| `Sources/SmartHome.MAX` | MAX bot integration. |
| `Sources/SmartHome.Host` | Host application that wires integrations together. |
| `Sources/SmartHome.Maui` | MAUI client application. |
| `Sources/SmartHome.Health` | Health-related module. |
| `Sources/SmartHome.Shared` | Shared SmartHome services. |

## Build And Run

From the repository root:

```powershell
dotnet restore Projects/Home/SmartHome/SmartHome.slnx
dotnet build Projects/Home/SmartHome/SmartHome.slnx
dotnet run --project Projects/Home/SmartHome/Sources/SmartHome.Host/SmartHome.Host.csproj
```

MAUI builds require MAUI workloads and the matching platform SDKs.
