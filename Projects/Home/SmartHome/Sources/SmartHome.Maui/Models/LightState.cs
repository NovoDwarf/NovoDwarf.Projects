using System.Text.Json.Serialization;

namespace SmartHome.Maui.Models;

public sealed record LightState(
    [property: JsonPropertyName("entityId")]    string EntityId,
    [property: JsonPropertyName("friendlyName")] string? FriendlyName,
    [property: JsonPropertyName("state")]       string State,
    [property: JsonPropertyName("brightness")]  int? Brightness,
    [property: JsonPropertyName("areaId")]      string AreaId,
    [property: JsonPropertyName("areaName")]    string AreaName)
{
    public bool IsOn => State == "on";
    public string DisplayName => FriendlyName ?? EntityId;
    public int BrightnessPercent => Brightness.HasValue ? (int)Math.Round(Brightness.Value / 255.0 * 100) : 0;
}
