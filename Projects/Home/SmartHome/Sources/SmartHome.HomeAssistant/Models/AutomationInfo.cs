namespace SmartHome.HomeAssistant.Models;

public sealed record AutomationInfo(
    string EntityId,
    string? FriendlyName,
    /// <summary>"on" = enabled, "off" = disabled.</summary>
    string State,
    /// <summary>UTC timestamp of the last trigger, or null if never triggered.</summary>
    DateTimeOffset? LastTriggered)
{
    public bool IsEnabled => State == "on";
}
