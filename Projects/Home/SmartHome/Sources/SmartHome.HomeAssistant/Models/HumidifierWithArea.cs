namespace SmartHome.HomeAssistant.Models;

public sealed record HumidifierWithArea(
    string EntityId,
    string? FriendlyName,
    string State,
    int? CurrentHumidity,
    int? TargetHumidity,
    string? Mode,
    IReadOnlyList<string> AvailableModes,
    string AreaId,
    string AreaName);
