namespace SmartHome.HomeAssistant.Models;

public sealed record MediaPlayerWithArea(
    string EntityId,
    string? FriendlyName,
    string State,
    double? Volume,
    bool IsMuted,
    string? Source,
    IReadOnlyList<string> Sources,
    string AreaId,
    string AreaName)
{
    public bool IsActive => State is "on" or "idle" or "playing" or "paused" or "buffering";
    public bool IsPlaying => State == "playing";
    public bool IsPaused => State == "paused";
    public int VolumePercent => Volume.HasValue ? (int)Math.Round(Volume.Value * 100) : 0;
}
