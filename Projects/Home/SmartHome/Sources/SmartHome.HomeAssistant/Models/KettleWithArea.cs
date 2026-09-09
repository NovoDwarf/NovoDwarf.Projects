namespace SmartHome.HomeAssistant.Models;

public sealed record KettleWithArea(
    string EntityId,
    string? FriendlyName,
    /// <summary>HA water_heater state = current operation mode ("off", "heat", "performance", …).</summary>
    string State,
    double? CurrentTemperature,
    double? TargetTemperature,
    double? MinTemp,
    double? MaxTemp,
    IReadOnlyList<string> OperationList,
    string AreaId,
    string AreaName)
{
    public bool IsOn => State != "off";
}
