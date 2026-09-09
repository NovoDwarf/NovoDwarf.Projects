using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartHome.MAX.Models;
using SmartHome.Shared;
using SmartHome.Shared.Commands;

namespace SmartHome.MAX;

public sealed class MaxBotService : BackgroundService
{
    private readonly MaxBotOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ICommandRouter _router;
    private readonly ILogger<MaxBotService> _logger;

    public MaxBotService(MaxBotOptions options,
        IHttpClientFactory httpClientFactory,
        ICommandRouter router,
        ILogger<MaxBotService> logger)
    {
        _options = options;
        _httpClientFactory = httpClientFactory;
        _router = router;
        _logger = logger;
    }

    private const int PollSeconds = 20;

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("max");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogMaxStarted(_options.BaseUrl);

        long lastEventId = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var url = $"/bot/v1/events?token={_options.Token}&lastEventId={lastEventId}&pollTime={PollSeconds}";
                var response = await CreateClient().GetFromJsonAsync<MaxEventsResponse>(url, stoppingToken);

                if (response?.Events is not { Length: > 0 } events) 
                    continue;
                
                foreach (var ev in events)
                {
                    lastEventId = Math.Max(lastEventId, ev.EventId);

                    if (ev is { Type: "newMessage", Payload: { Text: { } text } payload })
                        await HandleMessageAsync(payload, text, stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogPollingError(ex);
                await Task.Delay(5_000, stoppingToken);
            }
        }
    }

    private async Task HandleMessageAsync(MaxPayload payload, string text, CancellationToken ct)
    {
        var userId = payload.From?.UserId.ToString() ?? "";

        if (_options.AllowedUserIds.Length > 0 && !_options.AllowedUserIds.Contains(userId))
        {
            _logger.LogRejectedMessageFromUser(userId);
            return;
        }

        var chatId = payload.Chat?.ChatId ?? userId;
        _logger.LogMaxChatId(chatId, text);

        var context = new BotContext
        {
            ChatId = chatId,
            Text = text,
            ReplyAsync = (reply, token) => SendMessageAsync(chatId, reply, token)
        };

        try
        {
            await _router.RouteAsync(context, ct);
        }
        catch (Exception ex)
        {
            _logger.LogErrorHandlingMax(chatId, ex);
            await SendMessageAsync(chatId, "Произошла ошибка при выполнении команды.", ct);
        }
    }

    private async Task SendMessageAsync(string chatId, string text, CancellationToken ct)
    {
        await CreateClient().PostAsJsonAsync(
            $"/bot/v1/messages/sendText?token={_options.Token}",
            new { chatId, text },
            ct);
    }
}
