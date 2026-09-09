using Microsoft.Extensions.Logging;
using SmartHome.Shared;
using SmartHome.Shared.Commands;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartHome.Telegram;

/// <summary>
/// Dispatches incoming Telegram updates to the appropriate handler.
/// Text messages → command router or /home navigator.
/// Callback queries → /home navigator.
/// </summary>
internal sealed class TelegramUpdateHandler : IUpdateHandler
{
    private readonly TelegramBotOptions _options;
    private readonly TelegramHomeNavigator _navigator;
    private readonly ICommandRouter _router;
    private readonly ILogger<TelegramUpdateHandler> _logger;
    
    public TelegramUpdateHandler(TelegramBotOptions options,
        TelegramHomeNavigator navigator,
        ICommandRouter router,
        ILogger<TelegramUpdateHandler> logger)
    {
        _options = options;
        _navigator = navigator;
        _router = router;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.CallbackQuery is { } cb)
        {
            await _navigator.HandleCallbackAsync(cb, ct);
            return;
        }

        if (update.Message is not { Text: { } text } msg) return;

        var chatId = msg.Chat.Id;

        if (_options.AllowedChatIds.Length > 0 && !_options.AllowedChatIds.Contains(chatId))
        {
            _logger.LogRejectedMessage(chatId);
            return;
        }

        _logger.LogMessage(chatId, text);

        if (text.StartsWith("/home", StringComparison.OrdinalIgnoreCase))
        {
            await _navigator.SendHomeAsync(chatId, ct);
            return;
        }

        if (text.StartsWith("/app", StringComparison.OrdinalIgnoreCase))
        {
            await HandleAppCommandAsync(bot, chatId, ct);
            return;
        }

        var context = new BotContext
        {
            ChatId = chatId.ToString(),
            Text = text,
            ReplyAsync = async (reply, token) => await bot.SendMessage(chatId, reply, cancellationToken: token),
        };

        try
        {
            await _router.RouteAsync(context, ct);
        }
        catch (Exception ex)
        {
            _logger.LogErrorHandling(chatId, ex);
            await bot.SendMessage(chatId, "Произошла ошибка при выполнении команды.", cancellationToken: ct);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, HandleErrorSource source, CancellationToken ct)
    {
        _logger.LogPollingError(source, ex);
        return Task.CompletedTask;
    }

    private async Task HandleAppCommandAsync(ITelegramBotClient bot, long chatId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(_options.MiniAppUrl))
        {
            await bot.SendMessage(chatId, "Mini App не настроен.", cancellationToken: ct);
            return;
        }

        await bot.SendMessage(chatId,
            "Нажми кнопку, чтобы открыть панель управления:",
            replyMarkup: new InlineKeyboardMarkup([[
                InlineKeyboardButton.WithWebApp(
                    "🏠 Открыть SmartHome",
                    new WebAppInfo { Url = _options.MiniAppUrl })
            ]]),
            cancellationToken: ct);
    }
}
