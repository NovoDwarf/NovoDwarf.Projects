using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartHome.HomeAssistant.Models;

public sealed class HomeAssistantState
{
    [JsonPropertyName("entity_id")]
    public required string EntityId { get; init; }

    [JsonPropertyName("state")]
    public required string State { get; init; }

    [JsonPropertyName("attributes")]
    public Dictionary<string, JsonElement> Attributes { get; init; } = [];

    public string? FriendlyName =>
        Attributes.TryGetValue("friendly_name", out var v) ? v.GetString() : null;

    public int? Brightness =>
        Attributes.TryGetValue("brightness", out var v) && v.ValueKind == JsonValueKind.Number
            ? (int?)v.GetDouble()
            : null;

    public int? ColorTempMireds =>
        Attributes.TryGetValue("color_temp", out var v) && v.ValueKind == JsonValueKind.Number
            ? (int?)v.GetInt32()
            : null;
}
