namespace Mathematics.App.Maui.Systems.Sensors.Services;

public sealed class BarometerService
{
	public event EventHandler<double>? Reading;

	public static bool IsSupported => Barometer.Default.IsSupported;

	public void Start()
	{
		Barometer.Default.ReadingChanged += OnReading;
		Barometer.Default.Start(SensorSpeed.UI);
	}

	public void Stop()
	{
		Barometer.Default.ReadingChanged -= OnReading;
		Barometer.Default.Stop();
	}

	private void OnReading(object? sender, BarometerChangedEventArgs e)
	{
		Reading?.Invoke(this, e.Reading.PressureInHectopascals);
	}
}