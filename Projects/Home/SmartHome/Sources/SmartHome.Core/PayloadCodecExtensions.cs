namespace SmartHome.Core;

/// <summary>
/// Common encoding helpers: navigation and generic entity control.
/// Domain-specific extensions live next to their domain
/// (LightPayloadExtensions, HumidifierPayloadExtensions, etc.).
/// </summary>
public static class PayloadCodecExtensions
{
    // ── Navigation ────────────────────────────────────────────────────────────

    public static string Home(this IPayloadCodec codec) => codec.Encode(BotAction.Home());
    public static string Noop(this IPayloadCodec codec) => codec.Encode(BotAction.Noop());
    public static string Group(this IPayloadCodec codec, string group) => codec.Encode(BotAction.ToGroup(group));
    public static string Entity(this IPayloadCodec codec, string entityId) => codec.Encode(BotAction.ToEntity(entityId));

    // ── Generic entity control ────────────────────────────────────────────────

    public static string TurnOn(this IPayloadCodec codec, string entityId) => codec.Encode(BotAction.TurnOn(entityId));
    public static string TurnOff(this IPayloadCodec codec, string entityId) => codec.Encode(BotAction.TurnOff(entityId));
}
