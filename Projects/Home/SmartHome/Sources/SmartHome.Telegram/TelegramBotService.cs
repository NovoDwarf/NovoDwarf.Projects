using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SmartHome.Telegram;

/// <summary>
/// Starts the Telegram long-polling loop and registers bot commands.
/// All update handling is delegated to <see cref="TelegramUpdateHandler"/>.
/// </summary>
internal sealed class TelegramBotService(
    ITelegramBotClient            bot,
    TelegramBotOptions            options,
    TelegramUpdateHandler         handler,
    ILogger<TelegramBotService>   logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var me = await bot.GetMe(stoppingToken);
        logger.LogBotStarted(me.Username);

        await RegisterCommandsAsync(stoppingToken);

        bot.StartReceiving(
            handler,
            new ReceiverOptions { AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery] },
            stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task RegisterCommandsAsync(CancellationToken ct)
    {
        var commands = new List<BotCommand>
        {
            new() { Command = "home", Description = "Управление умным домом" },
            new() { Command = "help", Description = "Справка по командам" },
        };

        if (!string.IsNullOrWhiteSpace(options.MiniAppUrl))
            commands.Add(new BotCommand { Command = "app", Description = "Открыть панель управления" });

        await bot.SetMyCommands(commands, cancellationToken: ct);
    }
}
