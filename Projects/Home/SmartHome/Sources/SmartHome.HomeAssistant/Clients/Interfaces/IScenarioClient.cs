using SmartHome.HomeAssistant.Models;

namespace SmartHome.HomeAssistant.Clients.Interfaces;

/// <summary>
/// Provides access to HA automations and scripts.
/// On/Off for both is handled via the generic homeassistant domain (IEntityClient);
/// this interface adds scenario-specific operations only.
/// </summary>
public interface IScenarioClient
{
    Task<IReadOnlyList<AutomationInfo>> GetAutomationsAsync(CancellationToken ct = default);

    /// <summary>Runs the automation immediately, even if it is disabled.</summary>
    Task TriggerAutomationAsync(string entityId, CancellationToken ct = default);

    Task<IReadOnlyList<ScriptInfo>> GetScriptsAsync(CancellationToken ct = default);
}
