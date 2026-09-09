using System.Globalization;
using System.Text.Json.Serialization;

namespace SmartHome.Maui.Models;

public sealed record SensorState(
    [property: JsonPropertyName("entityId")]    string  EntityId,
    [property: JsonPropertyName("friendlyName")] string? FriendlyName,
    [property: JsonPropertyName("state")]       string  State,
    [property: JsonPropertyName("unit")]        string? Unit,
    [property: JsonPropertyName("deviceClass")] string? DeviceClass,
    [property: JsonPropertyName("areaId")]      string  AreaId,
    [property: JsonPropertyName("areaName")]    string  AreaName)
{
    public string DisplayName  => FriendlyName ?? EntityId;
    public string DisplayValue => Unit is not null ? $"{State} {Unit}" : State;

    public double? NumericValue => double.TryParse(
        State, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : null;
}

public sealed record BinarySensorState(
    [property: JsonPropertyName("entityId")]    string  EntityId,
    [property: JsonPropertyName("friendlyName")] string? FriendlyName,
    [property: JsonPropertyName("state")]       string  State,
    [property: JsonPropertyName("deviceClass")] string? DeviceClass,
    [property: JsonPropertyName("areaId")]      string  AreaId,
    [property: JsonPropertyName("areaName")]    string  AreaName)
{
    public bool   IsOn        => State == "on";
    public string DisplayName => FriendlyName ?? EntityId;
}
