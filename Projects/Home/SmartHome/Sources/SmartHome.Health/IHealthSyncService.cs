using SmartHome.Core;

namespace SmartHome.Health;

/// <summary>
/// Persists a health metrics snapshot to the backing store (Home Assistant virtual sensors).
/// </summary>
public interface IHealthSyncService
{
    Task SyncAsync(HealthMetrics metrics, CancellationToken ct = default);
}
