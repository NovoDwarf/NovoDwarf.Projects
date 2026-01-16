using Mathematics.App.Maui.UI.ViewModels.Utilities;

namespace Mathematics.App.Maui.Systems.Sensors.Services;

public sealed class RoundingService
{
	public double Apply(double value, RoundingMode mode)
	{
		return mode switch
		{
			RoundingMode.Up => Math.Ceiling(value),
			RoundingMode.Down => Math.Floor(value),
			RoundingMode.Nearest => Math.Round(value),
			_ => value
		};
	}
}