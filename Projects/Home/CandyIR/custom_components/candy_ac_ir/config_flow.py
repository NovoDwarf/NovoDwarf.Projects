"""Config flow for Candy AC IR."""
from __future__ import annotations

import voluptuous as vol
from homeassistant import config_entries
from homeassistant.const import CONF_HOST, CONF_NAME

DOMAIN = "candy_ac_ir"

STEP_USER_SCHEMA = vol.Schema({
    vol.Required(CONF_NAME, default="Candy AC"): str,
    vol.Required(CONF_HOST, default="http://slzb-ultima3.home.arpa"): str,
    vol.Optional("ir_endpoint", default="/api2?action=12"): str,
    vol.Optional("default_temp", default=22): vol.All(int, vol.Range(min=16, max=30)),
})


def _options_schema(config: dict) -> vol.Schema:
    return vol.Schema({
        vol.Required(CONF_HOST, default=config.get(CONF_HOST, "http://slzb-ultima3.home.arpa")): str,
        vol.Optional("ir_endpoint", default=config.get("ir_endpoint", "/api2?action=12")): str,
        vol.Optional("default_temp", default=config.get("default_temp", 22)): vol.All(
            int,
            vol.Range(min=16, max=30),
        ),
    })


class CandyACConfigFlow(config_entries.ConfigFlow, domain=DOMAIN):
    """Handle config flow for Candy AC IR."""

    VERSION = 1

    @staticmethod
    def async_get_options_flow(config_entry):
        return CandyACOptionsFlow(config_entry)

    async def async_step_user(self, user_input=None):
        errors = {}

        if user_input is not None:
            return self.async_create_entry(
                title=user_input[CONF_NAME],
                data=user_input,
            )

        return self.async_show_form(
            step_id="user",
            data_schema=STEP_USER_SCHEMA,
            errors=errors,
        )


class CandyACOptionsFlow(config_entries.OptionsFlow):
    """Handle options flow for Candy AC IR."""

    def __init__(self, config_entry):
        self._config_entry = config_entry

    async def async_step_init(self, user_input=None):
        if user_input is not None:
            return self.async_create_entry(title="", data=user_input)

        config = {**self._config_entry.data, **self._config_entry.options}
        return self.async_show_form(
            step_id="init",
            data_schema=_options_schema(config),
        )
