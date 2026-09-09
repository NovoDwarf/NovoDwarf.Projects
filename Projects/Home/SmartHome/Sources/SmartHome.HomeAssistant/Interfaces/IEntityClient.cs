namespace SmartHome.HomeAssistant.Interfaces;

/// <summary>Generic on/off that works for any entity via the homeassistant domain.</summary>
public interface IEntityClient
{
    public Task TurnOnAsync(string entityId, CancellationToken ct = default);
    public Task TurnOffAsync(string entityId, CancellationToken ct = default);
}
