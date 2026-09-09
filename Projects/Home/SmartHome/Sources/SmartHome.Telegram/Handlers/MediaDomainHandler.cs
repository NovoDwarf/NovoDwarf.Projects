using SmartHome.HomeAssistant;
using SmartHome.Core;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.Telegram.Keyboards;

namespace SmartHome.Telegram.Handlers;

internal sealed class MediaDomainHandler : IDomainHandler
{
    private readonly IMediaPlayerClient _ha;

    public MediaDomainHandler(IMediaPlayerClient ha)
    {
        _ha = ha;
    }

    public bool OwnsGroup(string group) => group == "media";
    public bool OwnsEntity(string entityId) => entityId.StartsWith("media_player.", StringComparison.Ordinal);
    public bool OwnsAction(string verb) => verb is "play" or "pause" or "vol" or "mute" or "src";

    public async Task<Screen> GetGroupScreenAsync(CancellationToken ct)
    {
        var players = await _ha.GetMediaPlayersWithAreasAsync(ct);
        return new Screen(MediaKeyboards.GroupText(players), MediaKeyboards.GroupKeyboard(players));
    }

    public async Task<Screen?> GetEntityScreenAsync(string entityId, CancellationToken ct)
    {
        var players = await _ha.GetMediaPlayersWithAreasAsync(ct);
        var player = players.FirstOrDefault(p => p.EntityId == entityId);
        return player is null ? null
            : new Screen(MediaKeyboards.EntityText(player), MediaKeyboards.EntityKeyboard(player));
    }

    public async Task<string?> ExecuteAsync(BotAction action, CancellationToken ct)
    {
        switch (action.Verb)
        {
            case "on":
                await _ha.TurnOnAsync(action.EntityId!, ct);
                return action.EntityId;
            case "off":
                await _ha.TurnOffAsync(action.EntityId!, ct);
                return action.EntityId;
            case "play":
                await _ha.MediaPlayAsync(action.EntityId!, ct);
                return action.EntityId;
            case "pause":
                await _ha.MediaPauseAsync(action.EntityId!, ct);
                return action.EntityId;
            case "vol" when action.Value.HasValue:
                await _ha.SetMediaVolumeAsync(action.EntityId!, action.Value.Value / 100.0, ct);
                return action.EntityId;
            case "mute" when action.State.HasValue:
                await _ha.SetMediaMuteAsync(action.EntityId!, action.State.Value == 1, ct);
                return action.EntityId;
            case "src" when action.Index.HasValue:
                var players = await _ha.GetMediaPlayersWithAreasAsync(ct);
                var player = players.FirstOrDefault(p => p.EntityId == action.EntityId);
                if (player is not null && action.Index.Value < player.Sources.Count)
                    await _ha.SelectMediaSourceAsync(action.EntityId!, player.Sources[action.Index.Value], ct);
                return action.EntityId;
        }
        return null;
    }
}
