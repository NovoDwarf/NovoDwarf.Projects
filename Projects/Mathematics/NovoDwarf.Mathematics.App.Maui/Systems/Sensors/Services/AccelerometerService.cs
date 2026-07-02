using System.Numerics;

namespace NovoDwarf.Mathematics.App.Systems.Sensors.Services;

public sealed class AccelerometerService
{
	public event EventHandler<Vector3>? Reading;

	public static bool IsSupported => Accelerometer.Default.IsSupported;

	public void Start()
	{
		Accelerometer.Default.ReadingChanged += OnReading;
		Accelerometer.Default.Start(SensorSpeed.UI);
	}

	public void Stop()
	{
		Accelerometer.Default.ReadingChanged -= OnReading;
		Accelerometer.Default.Stop();
	}

	private void OnReading(object? sender, AccelerometerChangedEventArgs e)
	{
		var a = e.Reading.Acceleration;
		
		Reading?.Invoke(this, new Vector3(a.X, a.Y, a.Z));
	}
}