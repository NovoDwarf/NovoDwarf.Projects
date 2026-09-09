using Microsoft.Extensions.Logging;
using Telegram.Bot.Polling;

namespace SmartHome.Telegram;

internal static partial class LoggerExtensions
{
    // ── TelegramBotService ────────────────────────────────────────────────────

    [LoggerMessage(LogLevel.Information, "Telegram bot @{Username} started")]
    public static partial void LogBotStarted(this ILogger<TelegramBotService> logger, string? username);

    // ── TelegramUpdateHandler ─────────────────────────────────────────────────

    [LoggerMessage(LogLevel.Warning, "Rejected message from chat {ChatId}")]
    public static partial void LogRejectedMessage(this ILogger<TelegramUpdateHandler> logger, long chatId);

    [LoggerMessage(LogLevel.Debug, "Telegram [{ChatId}]: {Text}")]
    public static partial void LogMessage(this ILogger<TelegramUpdateHandler> logger, long chatId, string text);

    [LoggerMessage(LogLevel.Error, "Error handling message from chat {ChatId}")]
    public static partial void LogErrorHandling(this ILogger<TelegramUpdateHandler> logger, long chatId, Exception exception);

    [LoggerMessage(LogLevel.Error, "Telegram polling error [{Source}]")]
    public static partial void LogPollingError(this ILogger<TelegramUpdateHandler> logger, HandleErrorSource source, Exception exception);

    // ── TelegramHomeNavigator ─────────────────────────────────────────────────

    [LoggerMessage(LogLevel.Debug, "Callback [{ChatId}]: {Data}")]
    public static partial void LogCallback(this ILogger<TelegramHomeNavigator> logger, long chatId, string? data);

    [LoggerMessage(LogLevel.Error, "Error handling callback {Data} for chat {ChatId}")]
    public static partial void LogErrorHandlingCallback(this ILogger<TelegramHomeNavigator> logger, string? data, long chatId, Exception exception);

    [LoggerMessage(LogLevel.Error, "Failed to send home screen to {ChatId}")]
    public static partial void LogFailedToSendHome(this ILogger<TelegramHomeNavigator> logger, long chatId, Exception exception);

    [LoggerMessage(LogLevel.Debug, "EditMessage skipped for msg {MessageId}: HA state not yet reflected")]
    public static partial void LogEditMessageSkipped(this ILogger<TelegramHomeNavigator> logger, int messageId);
}
