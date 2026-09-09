namespace SmartHome.Core;

/// <summary>
/// Converts <see cref="BotAction"/> to and from the bot-specific payload string.
/// One implementation per bot platform (Telegram, VK, Discord, …).
/// </summary>
public interface IPayloadCodec
{
    /// <summary>Encodes an action to the platform's callback-data string.</summary>
    string Encode(BotAction action);

    /// <summary>
    /// Decodes a raw callback-data string back to a <see cref="BotAction"/>.
    /// Returns <c>null</c> when the payload is not owned by this codec.
    /// </summary>
    BotAction? Decode(string? raw);
}
