using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Models;
using SmartHome.HomeAssistant.Utilities;

namespace SmartHome.HomeAssistant.Clients.Implementations;

internal sealed class HumidifierService : IHumidifierClient
{
    private readonly HaHttpContext _ctx;

    public HumidifierService(HaHttpContext ctx)
    {
        _ctx = ctx;
    }

    public Task TurnOnAsync(string entityId, CancellationToken ct = default)
        => _ctx.TurnOnAsync(entityId, ct);

    public Task TurnOffAsync(string entityId, CancellationToken ct = default)
        => _ctx.TurnOffAsync(entityId, ct);

    public async Task<IReadOnlyList<HumidifierWithArea>> GetHumidifiersWithAreasAsync(CancellationToken ct = default)
    {
        const string template =
            "{% set ns = namespace(r=[]) %}" +
            "{% for s in states.humidifier %}" +
            "{% set ns.r = ns.r + [{" +
            "\"entity_id\": s.entity_id," +
            "\"friendly_name\": s.name," +
            "\"state\": s.state," +
            "\"current_humidity\": s.attributes.current_humidity | default(none)," +
            "\"target_humidity\": s.attributes.humidity | default(none)," +
            "\"mode\": s.attributes.mode | default(none)," +
            "\"available_modes\": s.attributes.available_modes | default([])," +
            "\"area_id\": area_id(s.entity_id) | default(\"other\")," +
            "\"area_name\": area_name(s.entity_id) | default(\"Другое\")" +
            "}] %}" +
            "{% endfor %}" +
            "{{ ns.r | tojson }}";

        var json = await _ctx.RenderTemplateAsync(template, ct);
        var items = JsonSerializer.Deserialize<HumidifierAreaJson[]>(json) ?? [];

        return items.Select(i => new HumidifierWithArea(
            i.EntityId,
            i.FriendlyName,
            i.State,
            i.CurrentHumidity,
            i.TargetHumidity,
            i.Mode,
            i.AvailableModes ?? [],
            HaHttpContext.Coalesce(i.AreaId, "other"),
            HaHttpContext.Coalesce(i.AreaName, "Другое")
        )).ToList();
    }

    public async Task SetHumidifierHumidityAsync(string entityId, int targetHumidity, CancellationToken ct = default)
        => await _ctx.Http().PostAsJsonAsync("/api/services/humidifier/set_humidity",
            new { entity_id = entityId, humidity = targetHumidity }, ct);

    public async Task SetHumidifierModeAsync(string entityId, string mode, CancellationToken ct = default)
        => await _ctx.Http().PostAsJsonAsync("/api/services/humidifier/set_mode",
            new { entity_id = entityId, mode }, ct);

    private sealed record HumidifierAreaJson(
        [property: JsonPropertyName("entity_id")] string EntityId,
        [property: JsonPropertyName("friendly_name")] string? FriendlyName,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("current_humidity")] int? CurrentHumidity,
        [property: JsonPropertyName("target_humidity")] int? TargetHumidity,
        [property: JsonPropertyName("mode")] string? Mode,
        [property: JsonPropertyName("available_modes")] string[]? AvailableModes,
        [property: JsonPropertyName("area_id")] string? AreaId,
        [property: JsonPropertyName("area_name")] string? AreaName);
}
