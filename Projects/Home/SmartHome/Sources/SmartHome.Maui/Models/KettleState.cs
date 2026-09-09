using System.Text.Json.Serialization;

namespace SmartHome.Maui.Models;

public sealed record KettleState(
    [property: JsonPropertyName("entityId")]           string EntityId,
    [property: JsonPropertyName("friendlyName")]       string? FriendlyName,
    [property: JsonPropertyName("state")]              string State,
    [property: JsonPropertyName("currentTemperature")] double? CurrentTemperature,
    [property: JsonPropertyName("targetTemperature")]  double? TargetTemperature,
    [property: JsonPropertyName("areaId")]             string AreaId,
    [property: JsonPropertyName("areaName")]           string AreaName)
{
    public bool IsOn => State != "off";
    public string DisplayName => FriendlyName ?? EntityId;
}
