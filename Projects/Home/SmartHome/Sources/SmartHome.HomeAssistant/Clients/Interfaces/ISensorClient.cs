using SmartHome.HomeAssistant.Models;

namespace SmartHome.HomeAssistant.Clients.Interfaces;

public interface ISensorClient
{
    Task<IReadOnlyList<SensorInfo>>       GetSensorsAsync(CancellationToken ct = default);
    Task<IReadOnlyList<BinarySensorInfo>> GetBinarySensorsAsync(CancellationToken ct = default);
}
