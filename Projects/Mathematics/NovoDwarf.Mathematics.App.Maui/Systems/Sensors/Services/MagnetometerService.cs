using System.Numerics;

namespace NovoDwarf.Mathematics.App.Systems.Sensors.Services;

public class MagnetometerService
{
	public event EventHandler<Vector3>? Reading;
	
	public static bool IsSupported => Magnetometer.Default.IsSupported;

	public void Start()
	{
		Magnetometer.Default.ReadingChanged += OnReading;
		Magnetometer.Default.Start(SensorSpeed.UI);
	}

	public void Stop()
	{
		Magnetometer.Default.ReadingChanged -= OnReading;
		Magnetometer.Default.Stop();
	}

	private void OnReading(object? sender, MagnetometerChangedEventArgs e)
	{
		var a = e.Reading.MagneticField;
		
		Reading?.Invoke(this, new Vector3(a.X, a.Y, a.Z));
	}
}