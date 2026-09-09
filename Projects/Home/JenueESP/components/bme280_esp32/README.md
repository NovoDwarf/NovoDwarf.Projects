# BME280 ESP32 Component

This component adds ESP-IDF I2C driver support for the Bosch BME280 sensor API.

It is a fork of [`boschsensortec/BME280_SensorAPI`](https://github.com/boschsensortec/BME280_SensorAPI).

## Navigation

- [Repository root](../../../../../README.md)
- [Projects catalog](../../../../README.md)
- [Home](../../../README.md)

## Installation

To use the component in an ESP-IDF project:

1. Create a `components` directory in the main project directory.
2. Add this component under `components/bme280_esp32`.
3. Include the component from the project `CMakeLists.txt`.

When using Git, prefer a submodule or subtree if the component should stay connected to its upstream history.

For more details about ESP-IDF components, see the [ESP-IDF build system documentation](https://docs.espressif.com/projects/esp-idf/en/stable/esp32/api-guides/build-system.html).

## Sensor Overview

BME280 is a combined digital humidity, pressure, and temperature sensor. It is designed for low-power applications such as home automation, IoT devices, wearables, GPS enhancement, indoor navigation, outdoor navigation, weather forecasting, and vertical velocity estimation.

## Features

- Pressure measurement.
- Temperature measurement.
- Humidity measurement.
- ESP32 I2C integration.

## Useful Links

- [BME280 product page](https://www.bosch-sensortec.com/products/environmental-sensors/humidity-sensors-bme280/)
- [BME280 datasheet](https://www.bosch-sensortec.com/media/boschsensortec/downloads/datasheets/bst-bme280-ds002.pdf)
- [BME280 shuttle board flyer](https://www.bosch-sensortec.com/media/boschsensortec/downloads/shuttle_board_flyer/application_board_3_1/bst-bme280-sf000.pdf)
- [Bosch Sensortec community support](https://community.bosch-sensortec.com)
