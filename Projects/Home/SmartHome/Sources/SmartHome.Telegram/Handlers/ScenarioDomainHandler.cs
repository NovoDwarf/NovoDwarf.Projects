using SmartHome.Core;
using SmartHome.HomeAssistant.Clients.Interfaces;
using SmartHome.HomeAssistant.Interfaces;
using SmartHome.HomeAssistant.Models;
using SmartHome.Telegram.Keyboards;

namespace SmartHome.Telegram.Handlers;

internal sealed class ScenarioDomainHandler : IDomainHandler
{
    private readonly IScenarioClient _scenarios;
    private readonly IEntityClient _entity;

    public ScenarioDomainHandler(IScenarioClient scenarios, IEntityClient entity)
    {
        _scenarios = scenarios;
        _entity = entity;
    }

    public bool OwnsGroup(string group) => group == "scene";

    public bool OwnsEntity(string entityId) =>
        entityId.StartsWith("automation.", StringComparison.Ordinal) ||
        entityId.StartsWith("script.", StringComparison.Ordinal);

    public bool OwnsAction(string verb) => verb == "atrig";

    public async Task<Screen> GetGroupScreenAsync(CancellationToken ct)
    {
        var (automations, scripts) = await FetchAllAsync(ct);
        return new Screen(
            ScenarioKeyboards.GroupText(automations, scripts),
            ScenarioKeyboards.GroupKeyboard(automations, scripts));
    }

    public async Task<Screen?> GetEntityScreenAsync(string entityId, CancellationToken ct)
    {
        if (entityId.StartsWith("automation.", StringComparison.Ordinal))
        {
            var automations = await _scenarios.GetAutomationsAsync(ct);
            var a = automations.FirstOrDefault(x => x.EntityId == entityId);
            return a is null ? null
                : new Screen(ScenarioKeyboards.AutomationText(a), ScenarioKeyboards.AutomationKeyboard(a));
        }

        if (entityId.StartsWith("script.", StringComparison.Ordinal))
        {
            var scripts = await _scenarios.GetScriptsAsync(ct);
            var s = scripts.FirstOrDefault(x => x.EntityId == entityId);
            return s is null ? null
                : new Screen(ScenarioKeyboards.ScriptText(s), ScenarioKeyboards.ScriptKeyboard(s));
        }

        return null;
    }

    public async Task<string?> ExecuteAsync(BotAction action, CancellationToken ct)
    {
        var id = action.EntityId;
        if (id is null) return null;

        switch (action.Verb)
        {
            case "atrig":
                await _scenarios.TriggerAutomationAsync(id, ct);
                return id;

            // on/off for automations = enable/disable; for scripts = run/stop.
            // The homeassistant domain handles both correctly via IEntityClient.
            case "on":
                await _entity.TurnOnAsync(id, ct);
                return id;
            case "off":
                await _entity.TurnOffAsync(id, ct);
                return id;
        }

        return null;
    }

    private async Task<(IReadOnlyList<AutomationInfo>, IReadOnlyList<ScriptInfo>)> FetchAllAsync(CancellationToken ct)
    {
        var aTask = _scenarios.GetAutomationsAsync(ct);
        var sTask = _scenarios.GetScriptsAsync(ct);
        await Task.WhenAll(aTask, sTask);
        return (aTask.Result, sTask.Result);
    }
}
