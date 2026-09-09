#include <stdio.h>
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"
#include "esp_log.h"
#include "driver/i2c.h"
#include "common.h"

static const char *TAG = "bme280";

void startup_bme()
{
    ESP_ERROR_CHECK(i2c_driver_install(I2C_NUM_0, I2C_MODE_MASTER, 0, 0, 0));

    bme280_i2c_conf conf = {
        .addr = BME280_I2C_ADDR_PRIM,
        .i2c_port = I2C_NUM_0,
        .i2c_conf = {
            .mode = I2C_MODE_MASTER,
            .sda_io_num = GPIO_NUM_8,
            .sda_pullup_en = GPIO_PULLUP_ENABLE,
            .scl_io_num = GPIO_NUM_9,
            .scl_pullup_en = GPIO_PULLUP_ENABLE,
            .master.clk_speed = 400000,
        }
    };

    struct bme280_dev dev;
    uint32_t meas_time_us = 10000;

    bme280_init_i2c_dev(&conf, &dev);
    bme280_init(&dev);

    struct bme280_settings settings;
    bme280_get_sensor_settings(&settings, &dev);
    settings.filter       = BME280_FILTER_COEFF_2;
    settings.osr_h        = BME280_OVERSAMPLING_1X;
    settings.osr_p        = BME280_OVERSAMPLING_1X;
    settings.osr_t        = BME280_OVERSAMPLING_1X;
    settings.standby_time = BME280_STANDBY_TIME_0_5_MS;
    bme280_set_sensor_settings(BME280_SEL_ALL_SETTINGS, &settings, &dev);
    bme280_cal_meas_delay(&meas_time_us, &settings);

    ESP_LOGI(TAG, "BME280 готов, время измерения: %" PRIu32 " us", meas_time_us);

    struct bme280_data data;
    while (1) {
        bme280_set_sensor_mode(BME280_POWERMODE_FORCED, &dev);
        dev.delay_us(meas_time_us, dev.intf_ptr);
        bme280_get_sensor_data(BME280_ALL, &data, &dev);

        ESP_LOGI(TAG, "Температура: %.1f °C  Влажность: %.1f %%  Давление: %.1f hPa",
                 data.temperature, data.humidity, data.pressure / 100.0f);

        vTaskDelay(pdMS_TO_TICKS(2000));
    }
}

void app_main() {
    startup_bme();
}