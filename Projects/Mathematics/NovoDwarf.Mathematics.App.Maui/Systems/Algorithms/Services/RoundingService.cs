using NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

namespace NovoDwarf.Mathematics.App.Systems.Algorithms.Services;

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