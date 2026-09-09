using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Models;
using SmartHome.HomeAssistant.Utilities;

namespace SmartHome.HomeAssistant.Clients.Implementations;

internal sealed class KettleService : IKettleClient
{
    private readonly HaHttpContext _ctx;

    public KettleService(HaHttpContext ctx)
    {
        _ctx = ctx;
    }

    public Task TurnOnAsync(string entityId, CancellationToken ct = default)
        => _ctx.TurnOnAsync(entityId, ct);

    public Task TurnOffAsync(string entityId, CancellationToken ct = default)
        => _ctx.TurnOffAsync(entityId, ct);

    public async Task<IReadOnlyList<KettleWithArea>> GetKettlesWithAreasAsync(CancellationToken ct = default)
    {
        const string template =
            "{% set ns = namespace(r=[]) %}" +
            "{% for s in states.water_heater %}" +
            "{% set ns.r = ns.r + [{" +
            "\"entity_id\": s.entity_id," +
            "\"friendly_name\": s.name," +
            "\"state\": s.state," +
            "\"current_temperature\": s.attributes.current_temperature | default(none)," +
            "\"target_temperature\": s.attributes.temperature | default(none)," +
            "\"min_temp\": s.attributes.min_temp | default(none)," +
            "\"max_temp\": s.attributes.max_temp | default(none)," +
            "\"operation_list\": s.attributes.operation_list | default([])," +
            "\"area_id\": area_id(s.entity_id) | default(\"other\")," +
            "\"area_name\": area_name(s.entity_id) | default(\"Другое\")" +
            "}] %}" +
            "{% endfor %}" +
            "{{ ns.r | tojson }}";

        var json = await _ctx.RenderTemplateAsync(template, ct);
        var items = JsonSerializer.Deserialize<KettleAreaJson[]>(json) ?? [];

        return items.Select(i => new KettleWithArea(
            i.EntityId,
            i.FriendlyName,
            i.State,
            i.CurrentTemperature,
            i.TargetTemperature,
            i.MinTemp,
            i.MaxTemp,
            i.OperationList ?? [],
            HaHttpContext.Coalesce(i.AreaId, "other"),
            HaHttpContext.Coalesce(i.AreaName, "Другое")
        )).ToList();
    }

    public async Task SetKettleTemperatureAsync(string entityId, int temperature, CancellationToken ct = default)
        => await _ctx.Http().PostAsJsonAsync("/api/services/water_heater/set_temperature",
            new { entity_id = entityId, temperature }, ct);

    private sealed record KettleAreaJson(
        [property: JsonPropertyName("entity_id")]          string EntityId,
        [property: JsonPropertyName("friendly_name")]      string? FriendlyName,
        [property: JsonPropertyName("state")]              string State,
        [property: JsonPropertyName("current_temperature")] double? CurrentTemperature,
        [property: JsonPropertyName("target_temperature")] double? TargetTemperature,
        [property: JsonPropertyName("min_temp")]           double? MinTemp,
        [property: JsonPropertyName("max_temp")]           double? MaxTemp,
        [property: JsonPropertyName("operation_list")]     string[]? OperationList,
        [property: JsonPropertyName("area_id")]            string? AreaId,
        [property: JsonPropertyName("area_name")]          string? AreaName);
}
