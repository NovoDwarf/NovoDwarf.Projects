namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public sealed class CompassService
{
	public event EventHandler<double>? HeadingChanged;

	public static bool IsSupported => Compass.Default.IsSupported;

	public void Start()
	{
		Compass.Default.ReadingChanged += OnReading;
		Compass.Default.Start(SensorSpeed.UI);
	}

	public void Stop()
	{
		Compass.Default.ReadingChanged -= OnReading;
		Compass.Default.Stop();
	}

	private void OnReading(object? sender, CompassChangedEventArgs e)
	{
		HeadingChanged?.Invoke(this, e.Reading.HeadingMagneticNorth);
	}
}