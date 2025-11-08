using Mathematics.Distributions.Base;
using Mathematics.Transforms.Models;

namespace Mathematics.Distributions.Models.Continuous.SemiInfinite;

public class LevyDistribution : DistributionBase
{
	public LevyDistribution(double location, double scale)
	{
		Location = location;
		Scale = scale;
	}

	public double Location { get; }
	public double Scale { get; }

	public override double Calculate()
	{
		var (z, _) = BoxMullerTransform.Polar();

		return Location + Scale / (z * z);
	}

	public override double GetExpectedValue()
	{
		return double.PositiveInfinity;
	}

	public override double GetVariance()
	{
		return double.PositiveInfinity;
	}

	public override double GetMinValue()
	{
		return Location;
	}

	public override double GetMaxValue()
	{
		return double.PositiveInfinity;
	}

	public override string ToString()
	{
		return $"Levy [Location = {Location:F3}, Scale = {Scale:F3}]";
	}
}