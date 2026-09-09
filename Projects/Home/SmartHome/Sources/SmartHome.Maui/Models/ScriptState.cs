using System.Text.Json.Serialization;

namespace SmartHome.Maui.Models;

public sealed record ScriptState(
    [property: JsonPropertyName("entityId")]     string EntityId,
    [property: JsonPropertyName("friendlyName")] string? FriendlyName,
    [property: JsonPropertyName("state")]        string State)
{
    public bool IsRunning => State == "on";
    public string DisplayName => FriendlyName ?? EntityId;
}
