using SmartHome.HomeAssistant.Interfaces;
using SmartHome.HomeAssistant.Models;

namespace SmartHome.HomeAssistant.Clients.Interfaces;

public interface ILightClient : IEntityClient
{
    Task<IReadOnlyList<HomeAssistantState>> GetLightsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<LightWithArea>> GetLightsWithAreasAsync(CancellationToken ct = default);

    /// <summary>Turns the light on. Pass <paramref name="brightness"/> (0–255) to also set level.</summary>
    Task TurnLightOnAsync(string entityId, int? brightness = null, CancellationToken ct = default);
}
