namespace SmartHome.Telegram;

public sealed class TelegramBotOptions
{
    public required string Token { get; init; }

    public long[] AllowedChatIds { get; init; } = [];

    /// <summary>
    /// HTTPS URL of the Telegram Mini App.
    /// If set, the bot registers /app command and sends a WebApp button.
    /// Requires a public HTTPS endpoint (use ngrok/Cloudflare Tunnel for local dev).
    /// </summary>
    public string? MiniAppUrl { get; init; }
}
