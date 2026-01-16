using System.Numerics;

namespace Mathematics.App.Maui.Systems.Sensors.DTOs;

public sealed record BarometerData : IEqualityOperators<BarometerData, BarometerData, bool>
{
	public double Pressure { get; init; }
	public double Altitude { get; init; }
	public string Trend { get; init; } = "—";
}