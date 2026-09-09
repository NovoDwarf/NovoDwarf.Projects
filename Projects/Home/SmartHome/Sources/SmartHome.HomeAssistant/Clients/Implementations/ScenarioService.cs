using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Models;
using SmartHome.HomeAssistant.Utilities;

namespace SmartHome.HomeAssistant.Clients.Implementations;

internal sealed class ScenarioService : IScenarioClient
{
    private readonly HaHttpContext _ctx;

    public ScenarioService(HaHttpContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<IReadOnlyList<AutomationInfo>> GetAutomationsAsync(CancellationToken ct = default)
    {
        const string template =
            "{% set ns = namespace(r=[]) %}" +
            "{% for s in states.automation %}" +
            "{% set ns.r = ns.r + [{" +
            "\"entity_id\": s.entity_id," +
            "\"friendly_name\": s.name," +
            "\"state\": s.state," +
            "\"last_triggered\": s.attributes.last_triggered | default(none)" +
            "}] %}" +
            "{% endfor %}" +
            "{{ ns.r | tojson }}";

        var json = await _ctx.RenderTemplateAsync(template, ct);
        var items = JsonSerializer.Deserialize<AutomationJson[]>(json) ?? [];

        return items
            .OrderBy(i => i.FriendlyName ?? i.EntityId)
            .Select(i => new AutomationInfo(
                i.EntityId,
                i.FriendlyName,
                i.State,
                i.LastTriggered is { Length: > 0 } ts && DateTimeOffset.TryParse(ts, out var dt)
                    ? dt
                    : null))
            .ToList();
    }

    public async Task TriggerAutomationAsync(string entityId, CancellationToken ct = default)
        => await _ctx.Http().PostAsJsonAsync("/api/services/automation/trigger",
            new { entity_id = entityId }, ct);

    public async Task<IReadOnlyList<ScriptInfo>> GetScriptsAsync(CancellationToken ct = default)
    {
        const string template =
            "{% set ns = namespace(r=[]) %}" +
            "{% for s in states.script %}" +
            "{% set ns.r = ns.r + [{" +
            "\"entity_id\": s.entity_id," +
            "\"friendly_name\": s.name," +
            "\"state\": s.state" +
            "}] %}" +
            "{% endfor %}" +
            "{{ ns.r | tojson }}";

        var json = await _ctx.RenderTemplateAsync(template, ct);
        var items = JsonSerializer.Deserialize<ScriptJson[]>(json) ?? [];

        return items
            .OrderBy(i => i.FriendlyName ?? i.EntityId)
            .Select(i => new ScriptInfo(i.EntityId, i.FriendlyName, i.State))
            .ToList();
    }

    private sealed record AutomationJson(
        [property: JsonPropertyName("entity_id")]     string EntityId,
        [property: JsonPropertyName("friendly_name")] string? FriendlyName,
        [property: JsonPropertyName("state")]         string State,
        [property: JsonPropertyName("last_triggered")] string? LastTriggered);

    private sealed record ScriptJson(
        [property: JsonPropertyName("entity_id")]     string EntityId,
        [property: JsonPropertyName("friendly_name")] string? FriendlyName,
        [property: JsonPropertyName("state")]         string State);
}
