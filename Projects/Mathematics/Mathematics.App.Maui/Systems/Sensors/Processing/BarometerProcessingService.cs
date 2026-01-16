using BarometerData = Mathematics.App.Maui.Systems.Sensors.DTOs.BarometerData;

namespace Mathematics.App.Maui.Systems.Sensors.Processing;

public sealed class BarometerProcessingService
{
	private const double Alpha = 0.15;
	private const double P0 = 1013.25;

	private double _filtered;
	private readonly Queue<double> _history = [];

	public BarometerData Process(double raw)
	{
		_filtered += Alpha * (raw - _filtered);

		_history.Enqueue(_filtered);
		if (_history.Count > 20)
			_history.Dequeue();

		return new BarometerData
		{
			Pressure = _filtered,
			Altitude = CalcAltitude(_filtered),
			Trend = ResolveTrend()
		};
	}

	private static double CalcAltitude(double pressure) =>
		44330 * (1 - Math.Pow(pressure / P0, 0.1903));

	private string ResolveTrend()
	{
		if (_history.Count < 10)
			return "—";

		var arr = _history.ToArray();
		var delta = arr[^1] - arr[^10];

		return delta switch
		{
			> 0.05  => "↑",
			< -0.05 => "↓",
			_       => "→"
		};
	}
}