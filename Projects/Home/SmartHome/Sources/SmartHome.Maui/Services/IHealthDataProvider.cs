using SmartHome.Core;

namespace SmartHome.Maui.Services;

/// <summary>
/// Platform-specific source of health data.
/// iOS implementation reads from HealthKit; Android from Health Connect.
/// </summary>
public interface IHealthDataProvider
{
    /// <summary>True if health data is available on this device.</summary>
    bool IsAvailable { get; }

    /// <summary>Request user authorisation to read health data. Returns true if granted.</summary>
    Task<bool> RequestPermissionsAsync();

    /// <summary>Read today's health snapshot from the platform health store.</summary>
    Task<HealthMetrics> ReadTodayAsync();
}
