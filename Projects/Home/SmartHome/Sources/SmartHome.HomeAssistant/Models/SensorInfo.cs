using System.Globalization;

namespace SmartHome.HomeAssistant.Models;

public sealed record SensorInfo(
    string EntityId,
    string? FriendlyName,
    string State,
    string? Unit,
    string? DeviceClass,
    string AreaId,
    string AreaName)
{
    public string DisplayName  => FriendlyName ?? EntityId;
    public string DisplayValue => Unit is not null ? $"{State} {Unit}" : State;

    public double? NumericValue => double.TryParse(
        State, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : null;
}
