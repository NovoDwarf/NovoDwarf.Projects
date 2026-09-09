namespace SmartHome.HomeAssistant.Models;

public sealed record ScriptInfo(
    string EntityId,
    string? FriendlyName,
    /// <summary>"on" = currently running, "off" = idle.</summary>
    string State)
{
    public bool IsRunning => State == "on";
}
