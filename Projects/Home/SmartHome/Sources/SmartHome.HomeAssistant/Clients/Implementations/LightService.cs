using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Models;
using SmartHome.HomeAssistant.Utilities;

namespace SmartHome.HomeAssistant.Clients.Implementations;

internal sealed class LightService : ILightClient
{
    private readonly HaHttpContext _ctx;

    public LightService(HaHttpContext ctx)
    {
        _ctx = ctx;
    }

    public Task TurnOnAsync(string entityId, CancellationToken ct = default)
        => _ctx.TurnOnAsync(entityId, ct);

    public async Task<IReadOnlyList<HomeAssistantState>> GetLightsAsync(CancellationToken ct = default)
    {
        var states = await _ctx.Http().GetFromJsonAsync<HomeAssistantState[]>("/api/states", ct) ?? [];
        return states.Where(s => s.EntityId.StartsWith("light.", StringComparison.Ordinal)).ToArray();
    }

    public Task TurnOffAsync(string entityId, CancellationToken ct = default)
        => _ctx.TurnOffAsync(entityId, ct);

    public async Task<IReadOnlyList<LightWithArea>> GetLightsWithAreasAsync(CancellationToken ct = default)
    {
        const string template =
            "{% set ns = namespace(r=[]) %}" +
            "{% for s in states.light %}" +
            "{% set ns.r = ns.r + [{" +
            "\"entity_id\": s.entity_id," +
            "\"friendly_name\": s.name," +
            "\"state\": s.state," +
            "\"brightness\": s.attributes.brightness | default(none)," +
            "\"area_id\": area_id(s.entity_id) | default(\"other\")," +
            "\"area_name\": area_name(s.entity_id) | default(\"Другое\")" +
            "}] %}" +
            "{% endfor %}" +
            "{{ ns.r | tojson }}";

        var json = await _ctx.RenderTemplateAsync(template, ct);
        var items = JsonSerializer.Deserialize<LightAreaJson[]>(json) ?? [];

        return items.Select(i => new LightWithArea(
            i.EntityId,
            i.FriendlyName,
            i.State,
            i.Brightness.HasValue ? (int)i.Brightness.Value : null,
            HaHttpContext.Coalesce(i.AreaId, "other"),
            HaHttpContext.Coalesce(i.AreaName, "Другое")
        )).ToList();
    }

    public async Task TurnLightOnAsync(string entityId, int? brightness = null, CancellationToken ct = default)
    {
        var body = new Dictionary<string, object> { ["entity_id"] = entityId };
        if (brightness.HasValue) body["brightness"] = brightness.Value;
        await _ctx.Http().PostAsJsonAsync("/api/services/light/turn_on", body, ct);
    }

    private sealed record LightAreaJson(
        [property: JsonPropertyName("entity_id")] string EntityId,
        [property: JsonPropertyName("friendly_name")] string? FriendlyName,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("brightness")] double? Brightness,
        [property: JsonPropertyName("area_id")] string? AreaId,
        [property: JsonPropertyName("area_name")] string? AreaName);
}
