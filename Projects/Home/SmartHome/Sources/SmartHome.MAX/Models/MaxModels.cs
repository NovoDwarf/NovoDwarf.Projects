using System.Text.Json.Serialization;

namespace SmartHome.MAX.Models;

public sealed class MaxEventsResponse
{
    [JsonPropertyName("events")]
    public MaxEvent[] Events { get; init; } = [];
}

public sealed class MaxEvent
{
    [JsonPropertyName("eventId")]
    public long EventId { get; init; }

    [JsonPropertyName("type")]
    public string Type { get; init; } = "";

    [JsonPropertyName("payload")]
    public MaxPayload? Payload { get; init; }
}

public sealed class MaxPayload
{
    [JsonPropertyName("from")]
    public MaxUser? From { get; init; }

    [JsonPropertyName("chat")]
    public MaxChat? Chat { get; init; }

    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

public sealed class MaxUser
{
    [JsonPropertyName("userId")]
    public long UserId { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

public sealed class MaxChat
{
    [JsonPropertyName("chatId")]
    public string ChatId { get; init; } = "";

    [JsonPropertyName("type")]
    public string Type { get; init; } = "";
}
