/**
 * Copyright (C) 2020 Bosch Sensortec GmbH. All rights reserved.
 *
 * SPDX-License-Identifier: BSD-3-Clause
 */
/*
 *  Modified ESP32 to BME280 source code adopted from Bosch Sensortec BME280_SensorAPI
 */

#include <stdint.h>
#include <stdlib.h>
#include <stdio.h>
#include <string.h>

#include "rom/ets_sys.h"
#include "common.h"

/******************************************************************************/
/*!                Static variable definition                                 */

/*! timeout in the i2c peripheral for reads/writes */
const int timeout_ms = 500;
const TickType_t timeout = timeout_ms / portTICK_PERIOD_MS;
/******************************************************************************/
/*!                User interface functions                                   */

static void init_i2c(struct bme280_i2c_conf *conf);
/*!
 * I2C read function map to ESP32 platform
 */
BME280_INTF_RET_TYPE bme280_i2c_read(uint8_t reg_addr, uint8_t *reg_data, uint32_t length, void *intf_ptr)
{
    struct bme280_i2c_conf* intf = intf_ptr;
	init_i2c(intf);
	// write the reg address, then read the data out (device auto-increments)
	esp_err_t err = i2c_master_write_read_device(intf->i2c_port, intf->addr, &reg_addr, 1, reg_data, length, timeout);
	ESP_ERROR_CHECK(err);
	return err == ESP_OK ? BME280_INTF_RET_SUCCESS : BME280_E_COMM_FAIL;
}

/*!
 * I2C write function map to ESP32 platform
 */
BME280_INTF_RET_TYPE bme280_i2c_write(uint8_t reg_addr, const uint8_t *reg_data, uint32_t length, void *intf_ptr)
{
    struct bme280_i2c_conf* intf = intf_ptr;
	init_i2c(intf);
	// The API returns reg_addr and reg_data separately, even though the address is
	// part of transmit packet...
	//
	// Therefore we have to combine addr and data in a buffer
	//
	// BME280_MAX_LEN is currently 10; API upper limit on # of reg writes per TX
	// each register write is 2 bytes (1 byte address, 1 byte data)
	// max TX buffer is therefore 20 bytes, herein stack allocated
	uint8_t reg_addr_data[BME280_MAX_LEN * 2] = {0};
	assert(length < sizeof(reg_addr_data));
	reg_addr_data[0] = reg_addr;
	memcpy(&reg_addr_data[1], reg_data, length); 
	esp_err_t err = i2c_master_write_to_device(intf->i2c_port, intf->addr, reg_addr_data, length + 1, timeout);
	ESP_ERROR_CHECK(err);
	return err == ESP_OK ? BME280_INTF_RET_SUCCESS : BME280_E_COMM_FAIL;
}

/*!
 * Delay function map to ESP32 platform
 */
void bme280_delay_us(uint32_t period, void *intf_ptr)
{
	// replace this with a RTOS delay for multithreaded apps...
    ets_delay_us(period);
}

/*!
 * Build device with wrapped esp32 i2c fns for bme280 api
 */
void bme280_init_i2c_dev(struct bme280_i2c_conf *conf, struct bme280_dev *dev)
{
	// set up ESP wrapper function pointers
	dev->read = bme280_i2c_read;
	dev->write = bme280_i2c_write;
	dev->intf = BME280_I2C_INTF;
	dev->delay_us = bme280_delay_us;
	dev->intf_ptr = malloc(sizeof(struct bme280_dev));
	// deep copy ESP i2c config to device struct
	*(struct bme280_i2c_conf*)dev->intf_ptr = *conf;
}
/*!
 * Config i2c in esp32
 */
static void init_i2c(struct bme280_i2c_conf *conf)
{
	// config i2c, channel 0. used before every read/write operation.
	ESP_ERROR_CHECK(i2c_param_config(conf->i2c_port, &conf->i2c_conf));
}
