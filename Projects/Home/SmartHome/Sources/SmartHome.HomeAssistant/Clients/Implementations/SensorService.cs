using System.Text.Json;
using System.Text.Json.Serialization;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Models;
using SmartHome.HomeAssistant.Utilities;

namespace SmartHome.HomeAssistant.Clients.Implementations;

internal sealed class SensorService : ISensorClient
{
    private readonly HaHttpContext _ctx;

    public SensorService(HaHttpContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<SensorInfo>> GetSensorsAsync(CancellationToken ct = default)
    {
        const string template =
            "{% set ns = namespace(r=[]) %}" +
            "{% for s in states.sensor %}" +
            "{% set ns.r = ns.r + [{" +
            "\"entity_id\": s.entity_id," +
            "\"friendly_name\": s.name," +
            "\"state\": s.state," +
            "\"unit\": s.attributes.unit_of_measurement | default(none)," +
            "\"device_class\": s.attributes.device_class | default(none)," +
            "\"area_id\": area_id(s.entity_id) | default(\"other\")," +
            "\"area_name\": area_name(s.entity_id) | default(\"Другое\")" +
            "}] %}" +
            "{% endfor %}" +
            "{{ ns.r | tojson }}";

        var json  = await _ctx.RenderTemplateAsync(template, ct);
        var items = JsonSerializer.Deserialize<SensorJson[]>(json) ?? [];

        return items.Select(i => new SensorInfo(
            i.EntityId,
            i.FriendlyName,
            i.State,
            i.Unit,
            i.DeviceClass,
            HaHttpContext.Coalesce(i.AreaId,   "other"),
            HaHttpContext.Coalesce(i.AreaName, "Другое")
        )).ToList();
    }

    public async Task<IReadOnlyList<BinarySensorInfo>> GetBinarySensorsAsync(CancellationToken ct = default)
    {
        const string template =
            "{% set ns = namespace(r=[]) %}" +
            "{% for s in states.binary_sensor %}" +
            "{% set ns.r = ns.r + [{" +
            "\"entity_id\": s.entity_id," +
            "\"friendly_name\": s.name," +
            "\"state\": s.state," +
            "\"device_class\": s.attributes.device_class | default(none)," +
            "\"area_id\": area_id(s.entity_id) | default(\"other\")," +
            "\"area_name\": area_name(s.entity_id) | default(\"Другое\")" +
            "}] %}" +
            "{% endfor %}" +
            "{{ ns.r | tojson }}";

        var json  = await _ctx.RenderTemplateAsync(template, ct);
        var items = JsonSerializer.Deserialize<BinarySensorJson[]>(json) ?? [];

        return items.Select(i => new BinarySensorInfo(
            i.EntityId,
            i.FriendlyName,
            i.State,
            i.DeviceClass,
            HaHttpContext.Coalesce(i.AreaId,   "other"),
            HaHttpContext.Coalesce(i.AreaName, "Другое")
        )).ToList();
    }

    // ── Private DTOs ──────────────────────────────────────────────────────────

    private sealed record SensorJson(
        [property: JsonPropertyName("entity_id")]    string  EntityId,
        [property: JsonPropertyName("friendly_name")] string? FriendlyName,
        [property: JsonPropertyName("state")]        string  State,
        [property: JsonPropertyName("unit")]         string? Unit,
        [property: JsonPropertyName("device_class")] string? DeviceClass,
        [property: JsonPropertyName("area_id")]      string? AreaId,
        [property: JsonPropertyName("area_name")]    string? AreaName);

    private sealed record BinarySensorJson(
        [property: JsonPropertyName("entity_id")]    string  EntityId,
        [property: JsonPropertyName("friendly_name")] string? FriendlyName,
        [property: JsonPropertyName("state")]        string  State,
        [property: JsonPropertyName("device_class")] string? DeviceClass,
        [property: JsonPropertyName("area_id")]      string? AreaId,
        [property: JsonPropertyName("area_name")]    string? AreaName);
}
