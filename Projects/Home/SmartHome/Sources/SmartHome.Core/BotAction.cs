namespace SmartHome.Core;

/// <summary>
/// Platform-agnostic payload for a single bot UI action.
/// Contains only the data fields — domain-specific factories live next to their domain.
/// Bots encode/decode this via <see cref="IPayloadCodec"/>.
/// </summary>
public sealed record BotAction
{
    public required string Verb { get; init; }
    public string? EntityId { get; init; }
    public string? Group { get; init; }
    public int? Value { get; init; }
    public string? Mode { get; init; }
    public int? Index { get; init; }
    public int? State { get; init; }

    // ── Navigation (domain-agnostic UI) ───────────────────────────────────────

    public static BotAction Home() => new() { Verb = "home" };
    public static BotAction Noop() => new() { Verb = "noop" };
    public static BotAction ToGroup(string group) => new() { Verb = "g", Group = group };
    public static BotAction ToEntity(string entityId) => new() { Verb = "e", EntityId = entityId };

    // ── Generic entity control (shared by every domain) ───────────────────────

    public static BotAction TurnOn(string entityId) => new() { Verb = "on", EntityId = entityId };
    public static BotAction TurnOff(string entityId) => new() { Verb = "off", EntityId = entityId };
}
