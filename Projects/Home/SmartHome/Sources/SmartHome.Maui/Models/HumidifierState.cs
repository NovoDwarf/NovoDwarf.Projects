using System.Text.Json.Serialization;

namespace SmartHome.Maui.Models;

public sealed record HumidifierState(
    [property: JsonPropertyName("entityId")]        string EntityId,
    [property: JsonPropertyName("friendlyName")]    string? FriendlyName,
    [property: JsonPropertyName("state")]           string State,
    [property: JsonPropertyName("currentHumidity")] int? CurrentHumidity,
    [property: JsonPropertyName("targetHumidity")]  int? TargetHumidity,
    [property: JsonPropertyName("mode")]            string? Mode,
    [property: JsonPropertyName("availableModes")]  IReadOnlyList<string> AvailableModes,
    [property: JsonPropertyName("areaId")]          string AreaId,
    [property: JsonPropertyName("areaName")]        string AreaName)
{
    public bool IsOn => State == "on";
    public string DisplayName => FriendlyName ?? EntityId;
}
