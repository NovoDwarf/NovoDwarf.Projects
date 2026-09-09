using Microsoft.Extensions.Logging;

namespace SmartHome.MAX;

internal static partial class LoggerExtensions
{
    [LoggerMessage(LogLevel.Information, "MAX bot started: {BaseUrl}")]
    public static partial void LogMaxStarted(this ILogger<MaxBotService> logger, string baseUrl);

    [LoggerMessage(LogLevel.Error, "MAX bot polling error")]
    public static partial void LogPollingError(this ILogger<MaxBotService> logger, Exception exception);

    [LoggerMessage(LogLevel.Warning, "Rejected message from user {UserId}")]
    public static partial void LogRejectedMessageFromUser(this ILogger<MaxBotService> logger, string userId);

    [LoggerMessage(LogLevel.Debug, "MAX [{ChatId}]: {Text}")]
    public static partial void LogMaxChatId(this ILogger<MaxBotService> logger, string chatId, string text);

    [LoggerMessage(LogLevel.Error, "Error handling MAX message from {ChatId}")]
    public static partial void LogErrorHandlingMax(this ILogger<MaxBotService> logger, string chatId, Exception exception);
}
