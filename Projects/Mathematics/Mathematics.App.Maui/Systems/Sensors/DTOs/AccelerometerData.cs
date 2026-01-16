using System.Numerics;

namespace Mathematics.App.Maui.Systems.Sensors.DTOs;

public sealed record AccelerometerData : IEqualityOperators<AccelerometerData, AccelerometerData, bool>
{
	public Vector3 Linear { get; init; }
	public Vector3 Gravity { get; init; }
	public double LinearMagnitude { get; init; }
}