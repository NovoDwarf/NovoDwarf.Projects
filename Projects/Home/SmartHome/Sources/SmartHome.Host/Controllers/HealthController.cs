using Microsoft.AspNetCore.Mvc;
using SmartHome.Core;
using SmartHome.Health;
using SmartHome.Host.Filters;

namespace SmartHome.Host.Controllers;

[ApiController]
[Route("api/health")]
[ApiKeyAuth]
public sealed class HealthController(
    IHealthSyncService health,
    ILogger<HealthController> logger) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Post([FromBody] HealthMetrics metrics, CancellationToken ct)
    {
        await health.SyncAsync(metrics, ct);

        logger.LogInformation(
            "Health metrics received: steps={Steps} hr={HR} sleep={Sleep}h",
            metrics.Steps, metrics.HeartRate, metrics.SleepHours);

        return NoContent();
    }
}
