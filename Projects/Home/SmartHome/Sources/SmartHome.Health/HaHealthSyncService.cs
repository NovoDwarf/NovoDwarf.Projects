using SmartHome.Core;
using SmartHome.HomeAssistant.Interfaces;

namespace SmartHome.Health;

/// <summary>
/// Pushes each non-null health metric as a virtual sensor state into Home Assistant.
/// </summary>
internal sealed class HaHealthSyncService(IHaStateClient ha) : IHealthSyncService
{
    public async Task SyncAsync(HealthMetrics m, CancellationToken ct = default)
    {
        var tasks = new List<Task>(7);

        if (m.Steps.HasValue)
            tasks.Add(ha.SetStateAsync("sensor.healthkit_steps", m.Steps.Value.ToString(),
                new() { ["unit_of_measurement"] = "steps", ["friendly_name"] = "HealthKit: Шаги", ["icon"] = "mdi:walk" }, ct));

        if (m.HeartRate.HasValue)
            tasks.Add(ha.SetStateAsync("sensor.healthkit_heart_rate", m.HeartRate.Value.ToString("F0"),
                new() { ["unit_of_measurement"] = "bpm", ["friendly_name"] = "HealthKit: Пульс", ["icon"] = "mdi:heart-pulse" }, ct));

        if (m.RestingHeartRate.HasValue)
            tasks.Add(ha.SetStateAsync("sensor.healthkit_resting_heart_rate", m.RestingHeartRate.Value.ToString("F0"),
                new() { ["unit_of_measurement"] = "bpm", ["friendly_name"] = "HealthKit: Пульс в покое", ["icon"] = "mdi:heart" }, ct));

        if (m.SleepHours.HasValue)
            tasks.Add(ha.SetStateAsync("sensor.healthkit_sleep", m.SleepHours.Value.ToString("F1"),
                new() { ["unit_of_measurement"] = "h", ["friendly_name"] = "HealthKit: Сон", ["icon"] = "mdi:sleep" }, ct));

        if (m.ActiveCalories.HasValue)
            tasks.Add(ha.SetStateAsync("sensor.healthkit_active_calories", m.ActiveCalories.Value.ToString("F0"),
                new() { ["unit_of_measurement"] = "kcal", ["friendly_name"] = "HealthKit: Активные калории", ["icon"] = "mdi:fire" }, ct));

        if (m.Weight.HasValue)
            tasks.Add(ha.SetStateAsync("sensor.healthkit_weight", m.Weight.Value.ToString("F1"),
                new() { ["unit_of_measurement"] = "kg", ["friendly_name"] = "HealthKit: Вес", ["device_class"] = "weight", ["icon"] = "mdi:scale" }, ct));

        if (m.SpO2.HasValue)
            tasks.Add(ha.SetStateAsync("sensor.healthkit_spo2", m.SpO2.Value.ToString("F1"),
                new() { ["unit_of_measurement"] = "%", ["friendly_name"] = "HealthKit: SpO₂", ["icon"] = "mdi:blood-bag" }, ct));

        await Task.WhenAll(tasks);
    }
}
