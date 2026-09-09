using System.Text.Json.Serialization;

namespace SmartHome.Maui.Models;

public sealed record MediaPlayerState(
    [property: JsonPropertyName("entityId")]    string EntityId,
    [property: JsonPropertyName("friendlyName")] string? FriendlyName,
    [property: JsonPropertyName("state")]       string State,
    [property: JsonPropertyName("volume")]      double? Volume,
    [property: JsonPropertyName("isMuted")]     bool IsMuted,
    [property: JsonPropertyName("source")]      string? Source,
    [property: JsonPropertyName("sources")]     IReadOnlyList<string> Sources,
    [property: JsonPropertyName("areaId")]      string AreaId,
    [property: JsonPropertyName("areaName")]    string AreaName)
{
    public bool IsActive => State is "playing" or "paused" or "idle" or "buffering" or "on";
    public bool IsPlaying => State == "playing";
    public string DisplayName => FriendlyName ?? EntityId;
    public int VolumePercent => Volume.HasValue ? (int)Math.Round(Volume.Value * 100) : 0;
}
