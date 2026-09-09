namespace SmartHome.HomeAssistant.Models;

public sealed record LightWithArea(
    string EntityId,
    string? FriendlyName,
    string State,
    int? Brightness,
    string AreaId,
    string AreaName);
