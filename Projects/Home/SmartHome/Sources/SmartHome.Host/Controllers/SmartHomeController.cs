using Microsoft.AspNetCore.Mvc;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Interfaces;
using SmartHome.Host.Filters;

namespace SmartHome.Host.Controllers;

[ApiController]
[Route("api/smarthome")]
[ApiKeyAuth]
public sealed class SmartHomeController(
    ILightClient lights,
    IHumidifierClient humidifiers,
    IKettleClient kettles,
    IMediaPlayerClient media,
    IScenarioClient scenarios,
    IEntityClient entity,
    ISensorClient sensors) : ControllerBase
{
    // ── Lights ────────────────────────────────────────────────────────────────

    [HttpGet("lights")]
    public async Task<IActionResult> GetLights(CancellationToken ct)
        => Ok(await lights.GetLightsWithAreasAsync(ct));

    [HttpPost("lights/{entityId}/on")]
    public async Task<IActionResult> LightOn(string entityId, CancellationToken ct)
    {
        await entity.TurnOnAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("lights/{entityId}/off")]
    public async Task<IActionResult> LightOff(string entityId, CancellationToken ct)
    {
        await entity.TurnOffAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("lights/{entityId}/brightness/{value:int}")]
    public async Task<IActionResult> SetBrightness(string entityId, int value, CancellationToken ct)
    {
        await lights.TurnLightOnAsync(entityId, value, ct);
        return NoContent();
    }

    // ── Humidifiers ───────────────────────────────────────────────────────────

    [HttpGet("humidifiers")]
    public async Task<IActionResult> GetHumidifiers(CancellationToken ct)
        => Ok(await humidifiers.GetHumidifiersWithAreasAsync(ct));

    [HttpPost("humidifiers/{entityId}/on")]
    public async Task<IActionResult> HumidifierOn(string entityId, CancellationToken ct)
    {
        await entity.TurnOnAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("humidifiers/{entityId}/off")]
    public async Task<IActionResult> HumidifierOff(string entityId, CancellationToken ct)
    {
        await entity.TurnOffAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("humidifiers/{entityId}/humidity/{value:int}")]
    public async Task<IActionResult> SetHumidity(string entityId, int value, CancellationToken ct)
    {
        await humidifiers.SetHumidifierHumidityAsync(entityId, value, ct);
        return NoContent();
    }

    [HttpPost("humidifiers/{entityId}/mode/{mode}")]
    public async Task<IActionResult> SetHumidifierMode(string entityId, string mode, CancellationToken ct)
    {
        await humidifiers.SetHumidifierModeAsync(entityId, mode, ct);
        return NoContent();
    }

    // ── Kettles ───────────────────────────────────────────────────────────────

    [HttpGet("kettles")]
    public async Task<IActionResult> GetKettles(CancellationToken ct)
        => Ok(await kettles.GetKettlesWithAreasAsync(ct));

    [HttpPost("kettles/{entityId}/on")]
    public async Task<IActionResult> KettleOn(string entityId, CancellationToken ct)
    {
        await entity.TurnOnAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("kettles/{entityId}/off")]
    public async Task<IActionResult> KettleOff(string entityId, CancellationToken ct)
    {
        await entity.TurnOffAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("kettles/{entityId}/temperature/{value:int}")]
    public async Task<IActionResult> SetKettleTemperature(string entityId, int value, CancellationToken ct)
    {
        await kettles.SetKettleTemperatureAsync(entityId, value, ct);
        return NoContent();
    }

    // ── Media ─────────────────────────────────────────────────────────────────

    [HttpGet("media")]
    public async Task<IActionResult> GetMedia(CancellationToken ct)
        => Ok(await media.GetMediaPlayersWithAreasAsync(ct));

    [HttpPost("media/{entityId}/on")]
    public async Task<IActionResult> MediaOn(string entityId, CancellationToken ct)
    {
        await entity.TurnOnAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("media/{entityId}/off")]
    public async Task<IActionResult> MediaOff(string entityId, CancellationToken ct)
    {
        await entity.TurnOffAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("media/{entityId}/play")]
    public async Task<IActionResult> MediaPlay(string entityId, CancellationToken ct)
    {
        await media.MediaPlayAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("media/{entityId}/pause")]
    public async Task<IActionResult> MediaPause(string entityId, CancellationToken ct)
    {
        await media.MediaPauseAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("media/{entityId}/volume/{value:int}")]
    public async Task<IActionResult> SetVolume(string entityId, int value, CancellationToken ct)
    {
        await media.SetMediaVolumeAsync(entityId, value / 100.0, ct);
        return NoContent();
    }

    [HttpPost("media/{entityId}/mute/{muted:bool}")]
    public async Task<IActionResult> SetMute(string entityId, bool muted, CancellationToken ct)
    {
        await media.SetMediaMuteAsync(entityId, muted, ct);
        return NoContent();
    }

    [HttpPost("media/{entityId}/source/{idx:int}")]
    public async Task<IActionResult> SelectSource(string entityId, int idx, CancellationToken ct)
    {
        var players = await media.GetMediaPlayersWithAreasAsync(ct);
        var player = players.FirstOrDefault(p => p.EntityId == entityId);
        if (player is not null && idx >= 0 && idx < player.Sources.Count)
        {
            await media.SelectMediaSourceAsync(entityId, player.Sources[idx], ct);
        }
        return NoContent();
    }

    // ── Automations ───────────────────────────────────────────────────────────

    [HttpGet("automations")]
    public async Task<IActionResult> GetAutomations(CancellationToken ct)
        => Ok(await scenarios.GetAutomationsAsync(ct));

    [HttpPost("automations/{entityId}/trigger")]
    public async Task<IActionResult> TriggerAutomation(string entityId, CancellationToken ct)
    {
        await scenarios.TriggerAutomationAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("automations/{entityId}/on")]
    public async Task<IActionResult> AutomationOn(string entityId, CancellationToken ct)
    {
        await entity.TurnOnAsync(entityId, ct);
        return NoContent();
    }

    [HttpPost("automations/{entityId}/off")]
    public async Task<IActionResult> AutomationOff(string entityId, CancellationToken ct)
    {
        await entity.TurnOffAsync(entityId, ct);
        return NoContent();
    }

    // ── Scripts ───────────────────────────────────────────────────────────────

    [HttpGet("scripts")]
    public async Task<IActionResult> GetScripts(CancellationToken ct)
        => Ok(await scenarios.GetScriptsAsync(ct));

    [HttpPost("scripts/{entityId}/run")]
    public async Task<IActionResult> RunScript(string entityId, CancellationToken ct)
    {
        await entity.TurnOnAsync(entityId, ct);
        return NoContent();
    }

    // ── Sensors ───────────────────────────────────────────────────────────────

    [HttpGet("sensors")]
    public async Task<IActionResult> GetSensors(CancellationToken ct)
        => Ok(await sensors.GetSensorsAsync(ct));

    [HttpGet("sensors/binary")]
    public async Task<IActionResult> GetBinarySensors(CancellationToken ct)
        => Ok(await sensors.GetBinarySensorsAsync(ct));
}
