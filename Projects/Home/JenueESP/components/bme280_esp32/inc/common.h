/**\
 * Copyright (c) 2020 Bosch Sensortec GmbH. All rights reserved.
 *
 * SPDX-License-Identifier: BSD-3-Clause
 **/
/*
 *  Modified ESP32 to BME280 source code adopted from Bosch Sensortec BME280_SensorAPI
 */

#ifndef _COMMON_H
#define _COMMON_H

/*! CPP guard */
#ifdef __cplusplus
extern "C" {
#endif

#include <stdio.h>
#include "bme280.h"
#include "driver/i2c.h"

/***************************************************************************/

/*!                 User function prototypes
 ****************************************************************************/
/*
 * I2C config for a specific BME280 device
 */
typedef struct bme280_i2c_conf
{
	uint8_t addr; // BME280 device address
	i2c_port_t i2c_port; // Selected on-chip peripheral
	i2c_config_t i2c_conf; // ESP32 i2c config
} bme280_i2c_conf;
/*
 * Function for reading the sensor's registers through I2C bus.
 * Translates BME280 API call to ESP32 API
 */
BME280_INTF_RET_TYPE bme280_i2c_read(uint8_t reg_addr, uint8_t *reg_data, uint32_t length, void *intf_ptr);
/*
 * Function for writing the sensor's registers through I2C bus.
 * Translates BME280 API call to ESP32 API
 */
BME280_INTF_RET_TYPE bme280_i2c_write(uint8_t reg_addr, const uint8_t *reg_data, uint32_t length, void *intf_ptr);
/*
 * Function provides the delay for required time (Microsecond) 
 * Translates BME280 API call to ESP32 API
 */
void bme280_delay_us(uint32_t period_us, void *intf_ptr);

/*
 *  Function is to build the ESP32 I2C interface device for the bme280 api
 */
void bme280_init_i2c_dev(struct bme280_i2c_conf *conf, struct bme280_dev *dev_out);


#ifdef __cplusplus
}
#endif /* End of CPP guard */

#endif /* _COMMON_H */
