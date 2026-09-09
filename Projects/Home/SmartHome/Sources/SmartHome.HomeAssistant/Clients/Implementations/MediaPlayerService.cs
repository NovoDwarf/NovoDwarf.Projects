using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Models;
using SmartHome.HomeAssistant.Utilities;

namespace SmartHome.HomeAssistant.Clients.Implementations;

internal sealed class MediaPlayerService : IMediaPlayerClient
{
    private readonly HaHttpContext _ctx;

    public MediaPlayerService(HaHttpContext ctx)
    {
        _ctx = ctx;
    }

    public Task TurnOnAsync(string entityId, CancellationToken ct = default)
        => _ctx.TurnOnAsync(entityId, ct);

    public Task TurnOffAsync(string entityId, CancellationToken ct = default)
        => _ctx.TurnOffAsync(entityId, ct);

    public async Task<IReadOnlyList<MediaPlayerWithArea>> GetMediaPlayersWithAreasAsync(CancellationToken ct = default)
    {
        const string template =
            "{% set ns = namespace(r=[]) %}" +
            "{% for s in states.media_player %}" +
            "{% set ns.r = ns.r + [{" +
            "\"entity_id\": s.entity_id," +
            "\"friendly_name\": s.name," +
            "\"state\": s.state," +
            "\"volume_level\": s.attributes.volume_level | default(none)," +
            "\"is_volume_muted\": s.attributes.is_volume_muted | default(false)," +
            "\"source\": s.attributes.source | default(none)," +
            "\"source_list\": s.attributes.source_list | default([])," +
            "\"area_id\": area_id(s.entity_id) | default(\"other\")," +
            "\"area_name\": area_name(s.entity_id) | default(\"Другое\")" +
            "}] %}" +
            "{% endfor %}" +
            "{{ ns.r | tojson }}";

        var json = await _ctx.RenderTemplateAsync(template, ct);
        var items = JsonSerializer.Deserialize<MediaAreaJson[]>(json) ?? [];

        return items.Select(i => new MediaPlayerWithArea(
            i.EntityId,
            i.FriendlyName,
            i.State,
            i.VolumeLevel,
            i.IsVolumeMuted,
            i.Source,
            i.SourceList ?? [],
            HaHttpContext.Coalesce(i.AreaId, "other"),
            HaHttpContext.Coalesce(i.AreaName, "Другое")
        )).ToList();
    }

    public async Task MediaPlayAsync(string entityId, CancellationToken ct = default)
        => await _ctx.Http().PostAsJsonAsync("/api/services/media_player/media_play",
            new { entity_id = entityId }, ct);

    public async Task MediaPauseAsync(string entityId, CancellationToken ct = default)
        => await _ctx.Http().PostAsJsonAsync("/api/services/media_player/media_pause",
            new { entity_id = entityId }, ct);

    public async Task SetMediaVolumeAsync(string entityId, double volume, CancellationToken ct = default)
        => await _ctx.Http().PostAsJsonAsync("/api/services/media_player/volume_set",
            new { entity_id = entityId, volume_level = Math.Clamp(volume, 0.0, 1.0) }, ct);

    public async Task SetMediaMuteAsync(string entityId, bool muted, CancellationToken ct = default)
        => await _ctx.Http().PostAsJsonAsync("/api/services/media_player/volume_mute",
            new { entity_id = entityId, is_volume_muted = muted }, ct);

    public async Task SelectMediaSourceAsync(string entityId, string source, CancellationToken ct = default)
        => await _ctx.Http().PostAsJsonAsync("/api/services/media_player/select_source",
            new { entity_id = entityId, source }, ct);

    private sealed record MediaAreaJson(
        [property: JsonPropertyName("entity_id")] string EntityId,
        [property: JsonPropertyName("friendly_name")] string? FriendlyName,
        [property: JsonPropertyName("state")] string State,
        [property: JsonPropertyName("volume_level")] double? VolumeLevel,
        [property: JsonPropertyName("is_volume_muted")] bool IsVolumeMuted,
        [property: JsonPropertyName("source")] string? Source,
        [property: JsonPropertyName("source_list")] string[]? SourceList,
        [property: JsonPropertyName("area_id")] string? AreaId,
        [property: JsonPropertyName("area_name")] string? AreaName);
}
