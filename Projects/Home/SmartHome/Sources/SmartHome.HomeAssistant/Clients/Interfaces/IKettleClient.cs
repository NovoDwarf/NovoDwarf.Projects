using SmartHome.HomeAssistant.Interfaces;
using SmartHome.HomeAssistant.Models;

namespace SmartHome.HomeAssistant.Clients.Interfaces;

public interface IKettleClient : IEntityClient
{
    Task<IReadOnlyList<KettleWithArea>> GetKettlesWithAreasAsync(CancellationToken ct = default);
    Task SetKettleTemperatureAsync(string entityId, int temperature, CancellationToken ct = default);
}
