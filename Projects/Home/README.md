# Home

Home contains smart-home projects: the .NET SmartHome solution, a Home Assistant custom component for Candy AC IR control, and an ESP32 sensor project.

## Navigation

- [Repository root](../../README.md)
- [Projects catalog](../README.md)
- [Shared libraries](../../Shared/README.md)
- [SmartHome](SmartHome/README.md)
- [CandyIR](CandyIR/README.md)
- [JenueESP](JenueESP/README.md)
- [JenueESP BME280 component](JenueESP/components/bme280_esp32/README.md)

## Projects

| Project | Type | Purpose |
|---|---|---|
| [`SmartHome`](SmartHome/README.md) | .NET solution | Smart-home host, bot integrations, Home Assistant integration, shared services, health module, and MAUI client. |
| [`CandyIR`](CandyIR/README.md) | Home Assistant custom component | Candy AC climate entity controlled through an SLZB Ultima IR blaster. |
| [`JenueESP`](JenueESP/README.md) | ESP32/ESP-IDF, PlatformIO | Embedded project with a BME280 sensor component. |

## SmartHome

SmartHome has its own solution:

```powershell
dotnet restore Projects/Home/SmartHome/SmartHome.slnx
dotnet build Projects/Home/SmartHome/SmartHome.slnx
dotnet run --project Projects/Home/SmartHome/Sources/SmartHome.Host/SmartHome.Host.csproj
```

Main SmartHome projects:

| Project | Purpose |
|---|---|
| `SmartHome.Core` | Bot actions, payload codec, and shared domain model. |
| `SmartHome.HomeAssistant` | HTTP integration with Home Assistant. |
| `SmartHome.Telegram` | Telegram bot integration. |
| `SmartHome.VK` | VK bot integration. |
| `SmartHome.MAX` | MAX bot integration. |
| `SmartHome.Host` | Host application that composes integrations. |
| `SmartHome.Maui` | MAUI client application. |
| `SmartHome.Health` | Health-related integration module. |
| `SmartHome.Shared` | Shared SmartHome services. |

## CandyIR

See [`CandyIR/README.md`](CandyIR/README.md) for installation and IR protocol notes.

## JenueESP

`JenueESP` is an ESP32 project with `platformio.ini`, ESP-IDF `CMakeLists.txt`, and a BME280 component. See [`JenueESP/README.md`](JenueESP/README.md) for project details and [`JenueESP/components/bme280_esp32/README.md`](JenueESP/components/bme280_esp32/README.md) for component details.
