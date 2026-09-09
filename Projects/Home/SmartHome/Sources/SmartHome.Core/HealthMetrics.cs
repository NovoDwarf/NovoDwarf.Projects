using System.Text.Json.Serialization;

namespace SmartHome.Core;

/// <summary>
/// Health data snapshot sent from a mobile device to the backend.
/// All fields are optional — send only what the platform can provide.
/// </summary>
public sealed class HealthMetrics
{
    [JsonPropertyName("steps")]
    public int? Steps { get; init; }

    [JsonPropertyName("heartRate")]
    public double? HeartRate { get; init; }

    [JsonPropertyName("restingHeartRate")]
    public double? RestingHeartRate { get; init; }

    [JsonPropertyName("sleepHours")]
    public double? SleepHours { get; init; }

    [JsonPropertyName("activeCalories")]
    public double? ActiveCalories { get; init; }

    [JsonPropertyName("weight")]
    public double? Weight { get; init; }

    [JsonPropertyName("spo2")]
    public double? SpO2 { get; init; }
}
