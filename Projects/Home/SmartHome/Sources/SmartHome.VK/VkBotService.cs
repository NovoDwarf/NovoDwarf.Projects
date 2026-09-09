using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartHome.Core;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Extensions;
using SmartHome.HomeAssistant.Interfaces;
using SmartHome.Shared;
using SmartHome.Shared.Commands;
using VkNet.Abstractions;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace SmartHome.VK;

public sealed class VkBotService : BackgroundService
{
    private readonly IVkApi _vkApi;
    private readonly VkBotOptions _options;
    private readonly ICommandRouter _router;
    private readonly IEntityClient _entity;
    private readonly ILightClient _lights;
    private readonly IHumidifierClient _humidifiers;
    private readonly IMediaPlayerClient _media;
    private readonly IKettleClient _kettles;
    private readonly IScenarioClient _scenarios;
    private readonly ILogger<VkBotService> _logger;

    private const int PollWaitSeconds = 25;

    public VkBotService(
        IVkApi vkApi,
        VkBotOptions options,
        ICommandRouter router,
        IEntityClient entity,
        ILightClient lights,
        IHumidifierClient humidifiers,
        IMediaPlayerClient media,
        IKettleClient kettles,
        IScenarioClient scenarios,
        ILogger<VkBotService> logger)
    {
        _vkApi = vkApi;
        _options = options;
        _router = router;
        _entity = entity;
        _lights = lights;
        _humidifiers = humidifiers;
        _media = media;
        _kettles = kettles;
        _scenarios = scenarios;
        _logger = logger;
    }

    // ── Polling loop ──────────────────────────────────────────────────────────

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("VK bot started (group {GroupId})", _options.GroupId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try { await RunPollingSessionAsync(stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "VK polling session failed, retrying in 5 s");
                await Task.Delay(5_000, stoppingToken);
            }
        }
    }

    private async Task RunPollingSessionAsync(CancellationToken ct)
    {
        var serverInfo = await Task.Run(() => _vkApi.Groups.GetLongPollServer(_options.GroupId), ct);

        var key = serverInfo.Key;
        var server = serverInfo.Server;
        var ts = serverInfo.Ts;

        while (!ct.IsCancellationRequested)
        {
            BotsLongPollHistoryResponse history;
            try
            {
                history = await Task.Run(() =>
                    _vkApi.Groups.GetBotsLongPollHistory(new BotsLongPollHistoryParams
                    {
                        Key = key,
                        Server = server,
                        Ts = ts,
                        Wait = PollWaitSeconds,
                    }), ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogWarning(ex, "VK long poll error, refreshing server info");
                return;
            }

            ts = history.Ts;
            foreach (var update in history.Updates ?? [])
                await HandleUpdateAsync(update, ct);
        }
    }

    // ── Update dispatcher ─────────────────────────────────────────────────────

    private async Task HandleUpdateAsync(GroupUpdate update, CancellationToken ct)
    {
        switch (update.Type.Value)
        {
            case GroupUpdateType.MessageNew when update.Instance is MessageNew { Message: { } msg }:
                await HandleMessageAsync(msg, ct);
                break;

            case GroupUpdateType.MessageEvent when update.Instance is MessageEvent msgEvent:
                await HandleCallbackAsync(msgEvent, ct);
                break;
        }
    }

    // ── Text message handler ──────────────────────────────────────────────────

    private async Task HandleMessageAsync(Message msg, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(msg.Text)) return;

        var fromId = msg.FromId ?? 0;
        var peerId = msg.PeerId ?? fromId;

        if (_options.AllowedUserIds.Length > 0 && !_options.AllowedUserIds.Contains(fromId))
        {
            _logger.LogWarning("Rejected message from VK user {UserId}", fromId);
            return;
        }

        _logger.LogDebug("VK [{PeerId}] from {FromId}: {Text}", peerId, fromId, msg.Text);

        if (msg.Text.StartsWith("/home", StringComparison.OrdinalIgnoreCase))
        {
            var (text, kb) = VkKeyboards.Home();
            await SendMessageAsync(peerId, text, kb, ct);
            return;
        }

        var context = new BotContext
        {
            ChatId = peerId.ToString()!,
            Text = msg.Text,
            ReplyAsync = (reply, token) => SendMessageAsync(peerId, reply, null, token),
        };

        try { await _router.RouteAsync(context, ct); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error routing VK message from peer {PeerId}", peerId);
            await SendMessageAsync(peerId, "Произошла ошибка при выполнении команды.", null, ct);
        }
    }

    // ── Callback handler ──────────────────────────────────────────────────────

    private async Task HandleCallbackAsync(MessageEvent ev, CancellationToken ct)
    {
        // Acknowledge within 60 s — removes the loading spinner.
        try
        {
            await Task.Run(() =>
                _vkApi.Messages.SendMessageEventAnswer(
                    ev.EventId, ev.UserId ?? 0, ev.PeerId ?? 0, eventData: null), ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to ack VK callback {EventId}", ev.EventId);
        }

        var action = VkPayloadCodec.Instance.Decode(ev.Payload?.ToString());
        if (action is null) return;

        _logger.LogDebug("VK callback [{PeerId}] verb={Verb} id={EntityId}", ev.PeerId, action.Verb, action.EntityId);

        try
        {
            switch (action.Verb)
            {
                case "noop": break;

                case "home":
                    await EditAsync(ev, VkKeyboards.Home, ct);
                    break;

                case "g" when action.Group is not null:
                    await EditGroupAsync(ev, action.Group, ct);
                    break;

                case "e" when action.EntityId is not null:
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "on" when action.EntityId is not null:
                    await _entity.TurnOnAsync(action.EntityId, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "off" when action.EntityId is not null:
                    await _entity.TurnOffAsync(action.EntityId, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "bri" when action.EntityId is not null && action.Value.HasValue:
                    await _lights.TurnLightOnAsync(action.EntityId, action.Value, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "hum" when action.EntityId is not null && action.Value.HasValue:
                    await _humidifiers.SetHumidifierHumidityAsync(action.EntityId, action.Value.Value, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "mode" when action.EntityId is not null && action.Mode is not null:
                    await _humidifiers.SetHumidifierModeAsync(action.EntityId, action.Mode, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "play" when action.EntityId is not null:
                    await _media.MediaPlayAsync(action.EntityId, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "pause" when action.EntityId is not null:
                    await _media.MediaPauseAsync(action.EntityId, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "vol" when action.EntityId is not null && action.Value.HasValue:
                    await _media.SetMediaVolumeAsync(action.EntityId, action.Value.Value / 100.0, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "mute" when action.EntityId is not null && action.State.HasValue:
                    await _media.SetMediaMuteAsync(action.EntityId, action.State.Value == 1, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "src" when action.EntityId is not null && action.Index.HasValue:
                    await HandleSourceSelectAsync(ev, action.EntityId, action.Index.Value, ct);
                    break;

                case "ktemp" when action.EntityId is not null && action.Value.HasValue:
                    await _kettles.SetKettleTemperatureAsync(action.EntityId, action.Value.Value, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;

                case "atrig" when action.EntityId is not null:
                    await _scenarios.TriggerAutomationAsync(action.EntityId, ct);
                    await EditEntityAsync(ev, action.EntityId, ct);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling VK callback verb={Verb}", action.Verb);
        }
    }

    // ── Screen builders ───────────────────────────────────────────────────────

    private async Task EditGroupAsync(MessageEvent ev, string group, CancellationToken ct)
    {
        (string text, MessageKeyboard kb) screen = group switch
        {
            "light"  => VkKeyboards.LightGroup(await _lights.GetLightsWithAreasAsync(ct)),
            "climate" => VkKeyboards.HumidifierGroup(await _humidifiers.GetHumidifiersWithAreasAsync(ct)),
            "media"  => VkKeyboards.MediaGroup(await _media.GetMediaPlayersWithAreasAsync(ct)),
            "kettle" => VkKeyboards.KettleGroup(await _kettles.GetKettlesWithAreasAsync(ct)),
            "scene"  => await BuildSceneGroupAsync(ct),
            _ => VkKeyboards.Home(),
        };
        await EditMessageAsync(ev.PeerId, (ulong)(ev.ConversationMessageId ?? 0), screen.text, screen.kb, ct);
    }

    private async Task EditEntityAsync(MessageEvent ev, string entityId, CancellationToken ct)
    {
        (string text, MessageKeyboard kb) screen;

        if (entityId.StartsWith("light.", StringComparison.Ordinal))
        {
            var lights = await _lights.GetLightsWithAreasAsync(ct);
            var light = lights.FirstOrDefault(l => l.EntityId == entityId);
            screen = light is not null ? VkKeyboards.LightEntity(light) : VkKeyboards.Home();
        }
        else if (entityId.StartsWith("humidifier.", StringComparison.Ordinal))
        {
            var items = await _humidifiers.GetHumidifiersWithAreasAsync(ct);
            var item = items.FirstOrDefault(h => h.EntityId == entityId);
            screen = item is not null ? VkKeyboards.HumidifierEntity(item) : VkKeyboards.Home();
        }
        else if (entityId.StartsWith("media_player.", StringComparison.Ordinal))
        {
            var players = await _media.GetMediaPlayersWithAreasAsync(ct);
            var player = players.FirstOrDefault(p => p.EntityId == entityId);
            screen = player is not null ? VkKeyboards.MediaEntity(player) : VkKeyboards.Home();
        }
        else if (entityId.StartsWith("water_heater.", StringComparison.Ordinal))
        {
            var kettles = await _kettles.GetKettlesWithAreasAsync(ct);
            var kettle = kettles.FirstOrDefault(k => k.EntityId == entityId);
            screen = kettle is not null ? VkKeyboards.KettleEntity(kettle) : VkKeyboards.Home();
        }
        else if (entityId.StartsWith("automation.", StringComparison.Ordinal))
        {
            var automations = await _scenarios.GetAutomationsAsync(ct);
            var auto = automations.FirstOrDefault(a => a.EntityId == entityId);
            screen = auto is not null ? VkKeyboards.AutomationEntity(auto) : VkKeyboards.Home();
        }
        else if (entityId.StartsWith("script.", StringComparison.Ordinal))
        {
            var scripts = await _scenarios.GetScriptsAsync(ct);
            var script = scripts.FirstOrDefault(s => s.EntityId == entityId);
            screen = script is not null ? VkKeyboards.ScriptEntity(script) : VkKeyboards.Home();
        }
        else
        {
            screen = VkKeyboards.Home();
        }

        await EditMessageAsync(ev.PeerId, (ulong)(ev.ConversationMessageId ?? 0), screen.text, screen.kb, ct);
    }

    private async Task HandleSourceSelectAsync(MessageEvent ev, string entityId, int idx, CancellationToken ct)
    {
        var players = await _media.GetMediaPlayersWithAreasAsync(ct);
        var player = players.FirstOrDefault(p => p.EntityId == entityId);
        if (player is not null && idx < player.Sources.Count)
            await _media.SelectMediaSourceAsync(entityId, player.Sources[idx], ct);
        await EditEntityAsync(ev, entityId, ct);
    }

    private async Task<(string, MessageKeyboard)> BuildSceneGroupAsync(CancellationToken ct)
    {
        var aTask = _scenarios.GetAutomationsAsync(ct);
        var sTask = _scenarios.GetScriptsAsync(ct);
        await Task.WhenAll(aTask, sTask);
        return VkKeyboards.ScenarioGroup(aTask.Result, sTask.Result);
    }

    private Task EditAsync(MessageEvent ev, Func<(string, MessageKeyboard)> builder, CancellationToken ct)
    {
        var (text, kb) = builder();
        return EditMessageAsync(ev.PeerId, (ulong)(ev.ConversationMessageId ?? 0), text, kb, ct);
    }

    // ── VK API primitives ─────────────────────────────────────────────────────

    private async Task SendMessageAsync(long? peerId, string text, MessageKeyboard? keyboard, CancellationToken ct)
    {
        await Task.Run(() => _vkApi.Messages.Send(new MessagesSendParams
        {
            PeerId = peerId,
            Message = text,
            Keyboard = keyboard,
            RandomId = Random.Shared.Next(),
        }), ct);
    }

    private async Task EditMessageAsync(
        long? peerId, ulong convMsgId, string text, MessageKeyboard keyboard, CancellationToken ct)
    {
        try
        {
            await Task.Run(() => _vkApi.Messages.Edit(new MessageEditParams
            {
                PeerId = peerId ?? 0,
                ConversationMessageId = (long?)convMsgId,
                Message = text,
                Keyboard = keyboard,
            }), ct);
        }
        catch (Exception ex) when (
            ex.Message.Contains("can't edit this message", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("not modified", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogDebug("VK EditMessage skipped (content unchanged) for msg {MsgId}", convMsgId);
        }
    }
}
