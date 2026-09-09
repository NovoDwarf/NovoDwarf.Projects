namespace SmartHome.HomeAssistant.Models;

public sealed record BinarySensorInfo(
    string EntityId,
    string? FriendlyName,
    string State,
    string? DeviceClass,
    string AreaId,
    string AreaName)
{
    public bool   IsOn        => State == "on";
    public string DisplayName => FriendlyName ?? EntityId;
}
