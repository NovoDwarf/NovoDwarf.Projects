using System.Text.Json.Serialization;

namespace SmartHome.Maui.Models;

public sealed record AutomationState(
    [property: JsonPropertyName("entityId")]     string EntityId,
    [property: JsonPropertyName("friendlyName")] string? FriendlyName,
    [property: JsonPropertyName("state")]        string State,
    [property: JsonPropertyName("lastTriggered")] DateTimeOffset? LastTriggered)
{
    public bool IsEnabled => State == "on";
    public string DisplayName => FriendlyName ?? EntityId;
}
