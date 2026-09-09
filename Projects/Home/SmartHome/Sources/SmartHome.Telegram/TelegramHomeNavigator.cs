using Microsoft.Extensions.Logging;
using SmartHome.Core;
using SmartHome.Telegram.Handlers;
using SmartHome.Telegram.Keyboards;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace SmartHome.Telegram;

/// <summary>
/// Drives the /home inline-keyboard state machine.
/// Decodes Telegram callback data via <see cref="TelegramPayloadCodec"/>,
/// then routes the resulting <see cref="BotAction"/> to the correct
/// <see cref="IDomainHandler"/> — without knowing about specific domains.
/// </summary>
internal sealed class TelegramHomeNavigator
{
    private readonly IEnumerable<IDomainHandler> _handlers;
    private readonly ITelegramBotClient _bot;
    private readonly ILogger<TelegramHomeNavigator> _logger;

    public TelegramHomeNavigator(
        IEnumerable<IDomainHandler> handlers,
        ITelegramBotClient bot,
        ILogger<TelegramHomeNavigator> logger)
    {
        _handlers = handlers;
        _bot = bot;
        _logger = logger;
    }

    // ── Public entry points ───────────────────────────────────────────────────

    public async Task SendHomeAsync(long chatId, CancellationToken ct)
    {
        try
        {
            await _bot.SendMessage(chatId, HomeText(),
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: HomeKeyboard(),
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogFailedToSendHome(chatId, ex);
            await _bot.SendMessage(chatId, "Не удалось открыть панель управления.", cancellationToken: ct);
        }
    }

    public async Task HandleCallbackAsync(CallbackQuery cb, CancellationToken ct)
    {
        await _bot.AnswerCallbackQuery(cb.Id, cancellationToken: ct);
        if (cb.Message is not { } msg) return;

        var chatId = msg.Chat.Id;
        var messageId = msg.MessageId;

        var action = TelegramPayloadCodec.Instance.Decode(cb.Data);
        if (action is null) return;

        _logger.LogCallback(chatId, cb.Data);

        try
        {
            switch (action.Verb)
            {
                case "noop": break;

                case "home":
                    await SafeEditAsync(chatId, messageId, HomeScreen(), ct);
                    break;

                case "g" when action.Group is not null:
                    var gh = _handlers.FirstOrDefault(h => h.OwnsGroup(action.Group));
                    if (gh is not null)
                        await SafeEditAsync(chatId, messageId, await gh.GetGroupScreenAsync(ct), ct);
                    break;

                case "e" when action.EntityId is not null:
                    await RefreshEntityAsync(chatId, messageId, action.EntityId, ct);
                    break;

                case "on" or "off" when action.EntityId is not null:
                    var oh = _handlers.FirstOrDefault(h => h.OwnsEntity(action.EntityId));
                    if (oh is not null)
                        await oh.ExecuteAsync(action, ct);
                    await RefreshEntityAsync(chatId, messageId, action.EntityId, ct);
                    break;

                default:
                    if (action.EntityId is null) break;
                    var ah = _handlers.FirstOrDefault(h => h.OwnsAction(action.Verb));
                    if (ah is null) break;
                    var refreshId = await ah.ExecuteAsync(action, ct);
                    if (refreshId is not null)
                        await RefreshEntityAsync(chatId, messageId, refreshId, ct);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogErrorHandlingCallback(cb.Data, chatId, ex);
            await _bot.SendMessage(chatId, "⚠️ Не удалось выполнить команду.", cancellationToken: ct);
        }
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task RefreshEntityAsync(long chatId, int messageId, string entityId, CancellationToken ct)
    {
        var handler = _handlers.FirstOrDefault(h => h.OwnsEntity(entityId));
        var screen = handler is not null
            ? await handler.GetEntityScreenAsync(entityId, ct) ?? HomeScreen()
            : HomeScreen();
        await SafeEditAsync(chatId, messageId, screen, ct);
    }

    private async Task SafeEditAsync(long chatId, int messageId, Screen screen, CancellationToken ct)
    {
        try
        {
            await _bot.EditMessageText(
                chatId, messageId, screen.Text,
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: screen.Keyboard,
                cancellationToken: ct);
        }
        catch (ApiRequestException ex) when (ex.Message.Contains("message is not modified"))
        {
            _logger.LogEditMessageSkipped(messageId);
        }
    }

    // ── Home screen ───────────────────────────────────────────────────────────

    private static Screen HomeScreen() => new(HomeText(), HomeKeyboard());

    private static string HomeText() => "🏠 *SmartHome*\nВыберите раздел:";

    private static readonly IPayloadCodec Codec = TelegramPayloadCodec.Instance;

    private static InlineKeyboardMarkup HomeKeyboard() => new([
        [
            InlineKeyboardButton.WithCallbackData("💡 Свет",    Codec.Group("light")),
            InlineKeyboardButton.WithCallbackData("💧 Климат",  Codec.Group("climate")),
        ],
        [
            InlineKeyboardButton.WithCallbackData("📺 Медиа",   Codec.Group("media")),
            InlineKeyboardButton.WithCallbackData("☕ Чайники",  Codec.Group("kettle")),
        ],
        [
            InlineKeyboardButton.WithCallbackData("🤖 Сценарии", Codec.Group("scene")),
        ],
    ]);
}
