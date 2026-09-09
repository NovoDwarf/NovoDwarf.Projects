# JenueESP

JenueESP is an ESP32 project for home automation experiments. The current tree contains ESP-IDF/PlatformIO configuration and a BME280 sensor component.

## Navigation

- [Repository root](../../../README.md)
- [Projects catalog](../../README.md)
- [Home](../README.md)
- [BME280 component](components/bme280_esp32/README.md)

## Layout

| Path | Purpose |
|---|---|
| `platformio.ini` | PlatformIO project configuration. |
| `CMakeLists.txt` | ESP-IDF root build file. |
| `src/` | Application source code. |
| `components/bme280_esp32` | BME280 ESP32 component. |
| `include/`, `lib/`, `test/` | Standard PlatformIO support directories. |

## Build

Use PlatformIO or ESP-IDF from this directory:

```powershell
pio run
```

or use the matching ESP-IDF workflow for the configured ESP32 target.
