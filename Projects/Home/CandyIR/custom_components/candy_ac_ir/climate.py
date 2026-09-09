"""Candy AC IR - Climate entity for Home Assistant."""
from __future__ import annotations

import logging
from typing import Any

import aiohttp

from homeassistant.components.climate import (
    ClimateEntity,
    ClimateEntityFeature,
    HVACMode,
)
from homeassistant.components.climate.const import (
    FAN_AUTO,
    FAN_HIGH,
    FAN_LOW,
    FAN_MEDIUM,
    SWING_HORIZONTAL,
    SWING_OFF,
    SWING_ON,
)
from homeassistant.config_entries import ConfigEntry
from homeassistant.const import ATTR_TEMPERATURE, CONF_HOST, UnitOfTemperature
from homeassistant.core import HomeAssistant
from homeassistant.helpers.entity_platform import AddEntitiesCallback

from . import DOMAIN
from .ir_encoder import (
    FAN_AUTO as IR_FAN_AUTO,
    FAN_HIGH as IR_FAN_HIGH,
    FAN_LOW as IR_FAN_LOW,
    FAN_MED as IR_FAN_MED,
    MODE_AUTO,
    MODE_COOL,
    MODE_DRY,
    MODE_HEAT,
    cmd_jalousie_swing,
    cmd_power_off,
    cmd_power_on,
    cmd_set_fan,
    cmd_set_mode,
    cmd_set_temp,
    cmd_silent,
    cmd_sleep,
    cmd_turbo,
)

_LOGGER = logging.getLogger(__name__)

# HA fan mode -> IR fan byte
HA_FAN_TO_IR = {
    FAN_AUTO:   IR_FAN_AUTO,
    FAN_LOW:    IR_FAN_LOW,
    FAN_MEDIUM: IR_FAN_MED,
    FAN_HIGH:   IR_FAN_HIGH,
}
IR_FAN_TO_HA = {v: k for k, v in HA_FAN_TO_IR.items()}

# HA HVAC mode -> IR mode byte
HA_MODE_TO_IR = {
    HVACMode.AUTO: MODE_AUTO,
    HVACMode.COOL: MODE_COOL,
    HVACMode.DRY:  MODE_DRY,
    HVACMode.HEAT: MODE_HEAT,
    HVACMode.FAN_ONLY: MODE_AUTO,  # fan-only = auto mode, low fan
}
IR_MODE_TO_HA = {
    MODE_AUTO: HVACMode.AUTO,
    MODE_COOL: HVACMode.COOL,
    MODE_DRY:  HVACMode.DRY,
    MODE_HEAT: HVACMode.HEAT,
}

PRESET_NONE    = "none"
PRESET_SLEEP   = "sleep"
PRESET_ECO     = "eco"
PRESET_TURBO   = "turbo"
PRESET_SILENT  = "silent"


async def async_setup_entry(
    hass: HomeAssistant,
    entry: ConfigEntry,
    async_add_entities: AddEntitiesCallback,
) -> None:
    data = hass.data[DOMAIN][entry.entry_id]
    async_add_entities([CandyACClimate(data)], True)


class CandyACClimate(ClimateEntity):
    """Candy AC climate entity controlled via SLZB IR blaster."""

    _attr_has_entity_name = True
    _attr_name = None
    _attr_temperature_unit = UnitOfTemperature.CELSIUS
    _attr_min_temp = 16
    _attr_max_temp = 30
    _attr_target_temperature_step = 1

    _attr_hvac_modes = [
        HVACMode.OFF,
        HVACMode.AUTO,
        HVACMode.COOL,
        HVACMode.HEAT,
        HVACMode.DRY,
        HVACMode.FAN_ONLY,
    ]
    _attr_fan_modes = [FAN_AUTO, FAN_LOW, FAN_MEDIUM, FAN_HIGH]
    _attr_swing_modes = [SWING_OFF, SWING_ON]
    _attr_preset_modes = [PRESET_NONE, PRESET_SLEEP, PRESET_ECO,
                          PRESET_TURBO, PRESET_SILENT]

    _attr_supported_features = (
        ClimateEntityFeature.TARGET_TEMPERATURE
        | ClimateEntityFeature.FAN_MODE
        | ClimateEntityFeature.SWING_MODE
        | ClimateEntityFeature.PRESET_MODE
        | ClimateEntityFeature.TURN_ON
        | ClimateEntityFeature.TURN_OFF
    )

    def __init__(self, config: dict) -> None:
        self._config = config
        self._host = config[CONF_HOST].rstrip("/")
        self._endpoint = config.get("ir_endpoint", "/api2?action=12")
        self._default_temp: int = config.get("default_temp", 22)
        self._attr_unique_id = f"candy_ac_{self._host}"

        # State
        self._hvac_mode = HVACMode.OFF
        self._target_temp: float = self._default_temp
        self._fan_mode: str = FAN_AUTO
        self._swing_mode: str = SWING_OFF
        self._preset_mode: str = PRESET_NONE

    # ── Properties ───────────────────────────────────────────────────────────

    @property
    def hvac_mode(self) -> HVACMode:
        return self._hvac_mode

    @property
    def target_temperature(self) -> float:
        return self._target_temp

    @property
    def fan_mode(self) -> str:
        return self._fan_mode

    @property
    def swing_mode(self) -> str:
        return self._swing_mode

    @property
    def preset_mode(self) -> str:
        return self._preset_mode

    # ── IR sender ────────────────────────────────────────────────────────────

    async def _send_ir(self, raw_hex: str) -> None:
        url = f"{self._host}{self._endpoint}"
        payload = {"code": raw_hex}
        _LOGGER.debug("Sending IR command to %s: %s hex chars", url, len(raw_hex))
        try:
            async with aiohttp.ClientSession() as session:
                async with session.post(
                    url,
                    data=payload,
                    timeout=aiohttp.ClientTimeout(total=5),
                ) as resp:
                    response_text = await resp.text()
                    if resp.status != 200:
                        _LOGGER.error(
                            "SLZB returned %s for IR send: %s",
                            resp.status,
                            response_text[:200],
                        )
                    else:
                        _LOGGER.debug("SLZB accepted IR command: %s", response_text[:200])
        except aiohttp.ClientError as err:
            _LOGGER.error("Failed to send IR command: %s", err)

    def _ir_temp(self) -> int:
        return int(self._target_temp)

    def _ir_fan(self) -> int:
        return HA_FAN_TO_IR.get(self._fan_mode, IR_FAN_AUTO)

    def _ir_mode(self) -> int:
        return HA_MODE_TO_IR.get(self._hvac_mode, MODE_AUTO)

    # ── HA service handlers ───────────────────────────────────────────────────

    async def async_set_hvac_mode(self, hvac_mode: HVACMode) -> None:
        prev = self._hvac_mode
        self._hvac_mode = hvac_mode
        self._preset_mode = PRESET_NONE

        if hvac_mode == HVACMode.OFF:
            raw = cmd_power_off(temp=self._ir_temp(), fan=self._ir_fan())
        elif prev == HVACMode.OFF:
            # Was off → turn on
            raw = cmd_power_on(
                temp=self._ir_temp(),
                mode=self._ir_mode(),
                fan=self._ir_fan(),
            )
        else:
            raw = cmd_set_mode(
                mode=self._ir_mode(),
                temp=self._ir_temp(),
                fan=self._ir_fan(),
            )

        await self._send_ir(raw)
        self.async_write_ha_state()

    async def async_set_temperature(self, **kwargs: Any) -> None:
        temp = kwargs.get(ATTR_TEMPERATURE)
        if temp is None:
            return
        self._target_temp = temp
        if self._hvac_mode == HVACMode.OFF:
            return
        raw = cmd_set_temp(
            temp=self._ir_temp(),
            mode=self._ir_mode(),
            fan=self._ir_fan(),
            power=True,
        )
        await self._send_ir(raw)
        self.async_write_ha_state()

    async def async_set_fan_mode(self, fan_mode: str) -> None:
        self._fan_mode = fan_mode
        if self._hvac_mode == HVACMode.OFF:
            return
        raw = cmd_set_fan(
            fan=self._ir_fan(),
            temp=self._ir_temp(),
            mode=self._ir_mode(),
        )
        await self._send_ir(raw)
        self.async_write_ha_state()

    async def async_set_swing_mode(self, swing_mode: str) -> None:
        self._swing_mode = swing_mode
        if self._hvac_mode == HVACMode.OFF:
            return
        raw = cmd_jalousie_swing(
            temp=self._ir_temp(),
            mode=self._ir_mode(),
            fan=self._ir_fan(),
        )
        await self._send_ir(raw)
        self.async_write_ha_state()

    async def async_set_preset_mode(self, preset_mode: str) -> None:
        self._preset_mode = preset_mode
        if self._hvac_mode == HVACMode.OFF:
            return

        if preset_mode == PRESET_SLEEP:
            raw = cmd_sleep(temp=self._ir_temp(), fan=self._ir_fan())
        elif preset_mode == PRESET_SILENT:
            raw = cmd_silent(
                temp=self._ir_temp(),
                mode=self._ir_mode(),
                fan=IR_FAN_LOW,
            )
        elif preset_mode == PRESET_TURBO:
            raw = cmd_turbo(
                temp=self._ir_temp(),
                mode=self._ir_mode(),
                fan=IR_FAN_LOW,
            )
        else:
            # PRESET_NONE / PRESET_ECO → send normal set_temp to cancel
            raw = cmd_set_temp(
                temp=self._ir_temp(),
                mode=self._ir_mode(),
                fan=self._ir_fan(),
            )

        await self._send_ir(raw)
        self.async_write_ha_state()

    async def async_turn_on(self) -> None:
        await self.async_set_hvac_mode(
            self._hvac_mode if self._hvac_mode != HVACMode.OFF else HVACMode.AUTO
        )

    async def async_turn_off(self) -> None:
        await self.async_set_hvac_mode(HVACMode.OFF)
