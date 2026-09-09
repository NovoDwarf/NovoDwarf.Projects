using SmartHome.HomeAssistant;
using SmartHome.Core;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.Telegram.Keyboards;

namespace SmartHome.Telegram.Handlers;

internal sealed class LightDomainHandler : IDomainHandler
{
    private readonly ILightClient _ha;

    public LightDomainHandler(ILightClient ha)
    {
        _ha = ha;
    }

    public bool OwnsGroup(string group) => group == "light";
    public bool OwnsEntity(string entityId) => entityId.StartsWith("light.", StringComparison.Ordinal);
    public bool OwnsAction(string verb) => verb == "bri";

    public async Task<Screen> GetGroupScreenAsync(CancellationToken ct)
    {
        var lights = await _ha.GetLightsWithAreasAsync(ct);
        return new Screen(LightKeyboards.GroupText(lights), LightKeyboards.GroupKeyboard(lights));
    }

    public async Task<Screen?> GetEntityScreenAsync(string entityId, CancellationToken ct)
    {
        var lights = await _ha.GetLightsWithAreasAsync(ct);
        var light = lights.FirstOrDefault(l => l.EntityId == entityId);
        return light is null ? null
            : new Screen(LightKeyboards.EntityText(light), LightKeyboards.EntityKeyboard(light));
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
            case "bri" when action.Value.HasValue:
                await _ha.TurnLightOnAsync(action.EntityId!, action.Value, ct);
                return action.EntityId;
        }
        return null;
    }
}
