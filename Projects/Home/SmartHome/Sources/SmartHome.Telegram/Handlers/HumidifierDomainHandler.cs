using SmartHome.HomeAssistant;
using SmartHome.Core;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.Telegram.Keyboards;

namespace SmartHome.Telegram.Handlers;

internal sealed class HumidifierDomainHandler : IDomainHandler
{
    private readonly IHumidifierClient _ha;

    public HumidifierDomainHandler(IHumidifierClient ha)
    {
        _ha = ha;
    }

    public bool OwnsGroup(string group) => group == "climate";
    public bool OwnsEntity(string entityId) => entityId.StartsWith("humidifier.", StringComparison.Ordinal);
    public bool OwnsAction(string verb) => verb is "hum" or "mode";

    public async Task<Screen> GetGroupScreenAsync(CancellationToken ct)
    {
        var items = await _ha.GetHumidifiersWithAreasAsync(ct);
        return new Screen(HumidifierKeyboards.GroupText(items), HumidifierKeyboards.GroupKeyboard(items));
    }

    public async Task<Screen?> GetEntityScreenAsync(string entityId, CancellationToken ct)
    {
        var items = await _ha.GetHumidifiersWithAreasAsync(ct);
        var item = items.FirstOrDefault(h => h.EntityId == entityId);
        return item is null ? null
            : new Screen(HumidifierKeyboards.EntityText(item), HumidifierKeyboards.EntityKeyboard(item));
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
            case "hum" when action.Value.HasValue:
                await _ha.SetHumidifierHumidityAsync(action.EntityId!, action.Value.Value, ct);
                return action.EntityId;
            case "mode" when action.Mode is not null:
                await _ha.SetHumidifierModeAsync(action.EntityId!, action.Mode, ct);
                return action.EntityId;
        }
        return null;
    }
}
