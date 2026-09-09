using SmartHome.HomeAssistant.Interfaces;
using SmartHome.HomeAssistant.Models;

namespace SmartHome.HomeAssistant.Clients.Interfaces;

public interface IHumidifierClient : IEntityClient
{
    Task<IReadOnlyList<HumidifierWithArea>> GetHumidifiersWithAreasAsync(CancellationToken ct = default);
    Task SetHumidifierHumidityAsync(string entityId, int targetHumidity, CancellationToken ct = default);
    Task SetHumidifierModeAsync(string entityId, string mode, CancellationToken ct = default);
}
