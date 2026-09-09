namespace SmartHome.HomeAssistant.Interfaces;

/// <summary>
/// Low-level HA REST state writer — for pushing virtual sensor values
/// that don't belong to any specific device domain.
/// </summary>
public interface IHaStateClient
{
    Task SetStateAsync(
        string entityId,
        string state,
        Dictionary<string, object>? attributes = null,
        CancellationToken ct = default);
}
