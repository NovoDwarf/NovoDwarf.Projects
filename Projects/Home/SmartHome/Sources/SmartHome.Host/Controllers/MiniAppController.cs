using Microsoft.AspNetCore.Mvc;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Interfaces;
using SmartHome.Host.Filters;

namespace SmartHome.Host.Controllers;

[ApiController]
[Route("api/mini-app")]
[TelegramInitData]
public sealed class MiniAppController : ControllerBase
{
    private readonly ILightClient _lights;
    private readonly IEntityClient _entity;

    public MiniAppController(ILightClient lights, IEntityClient entity)
    {
        _lights = lights;
        _entity = entity;
    }

    public sealed record LightDto(
        string EntityId,
        string? FriendlyName,
        string State,
        int? Brightness,
        int? ColorTempMireds);

    public sealed record BrightnessRequest(int Brightness);
    
    /// <summary>Returns all light entities with their current state.</summary>
    [HttpGet("lights")]
    [ProducesResponseType<LightDto[]>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLights(CancellationToken ct)
    {
        var lights = await _lights.GetLightsAsync(ct);
        var dtos = lights.Select(l => new LightDto(
            l.EntityId,
            l.FriendlyName,
            l.State,
            l.Brightness,
            l.ColorTempMireds));
        return Ok(dtos);
    }

    /// <summary>Turns a light on.</summary>
    [HttpPost("lights/{entityId}/on")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> TurnOn(string entityId, CancellationToken ct)
    {
        await _lights.TurnLightOnAsync(entityId, ct: ct);
        return NoContent();
    }

    /// <summary>Turns a light off.</summary>
    [HttpPost("lights/{entityId}/off")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> TurnOff(string entityId, CancellationToken ct)
    {
        await _entity.TurnOffAsync(entityId, ct);
        return NoContent();
    }

    /// <summary>Sets brightness (0–255). Turns the light on if it is off.</summary>
    [HttpPost("lights/{entityId}/brightness")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetBrightness(
        string entityId,
        [FromBody] BrightnessRequest req,
        CancellationToken ct)
    {
        var brightness = Math.Clamp(req.Brightness, 0, 255);
        await _lights.TurnLightOnAsync(entityId, brightness, ct);
        return NoContent();
    }
}
