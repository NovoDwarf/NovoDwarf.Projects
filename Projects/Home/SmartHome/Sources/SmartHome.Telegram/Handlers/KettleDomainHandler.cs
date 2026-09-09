using SmartHome.Core;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.Telegram.Keyboards;

namespace SmartHome.Telegram.Handlers;

internal sealed class KettleDomainHandler : IDomainHandler
{
    private readonly IKettleClient _ha;

    public KettleDomainHandler(IKettleClient ha)
    {
        _ha = ha;
    }

    public bool OwnsGroup(string group) => group == "kettle";
    public bool OwnsEntity(string entityId) => entityId.StartsWith("water_heater.", StringComparison.Ordinal);
    public bool OwnsAction(string verb) => verb == "ktemp";

    public async Task<Screen> GetGroupScreenAsync(CancellationToken ct)
    {
        var items = await _ha.GetKettlesWithAreasAsync(ct);
        return new Screen(KettleKeyboards.GroupText(items), KettleKeyboards.GroupKeyboard(items));
    }

    public async Task<Screen?> GetEntityScreenAsync(string entityId, CancellationToken ct)
    {
        var items = await _ha.GetKettlesWithAreasAsync(ct);
        var item = items.FirstOrDefault(k => k.EntityId == entityId);
        return item is null ? null
            : new Screen(KettleKeyboards.EntityText(item), KettleKeyboards.EntityKeyboard(item));
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
            case "ktemp" when action.Value.HasValue:
                await _ha.SetKettleTemperatureAsync(action.EntityId!, action.Value.Value, ct);
                return action.EntityId;
        }
        return null;
    }
}
