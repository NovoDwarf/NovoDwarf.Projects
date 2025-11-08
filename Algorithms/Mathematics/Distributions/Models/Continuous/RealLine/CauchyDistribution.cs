using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Continuous.RealLine;

public class CauchyDistribution : DistributionBase
{
	public CauchyDistribution(double location, double scale)
	{
		Location = location;
		Scale = scale;
	}

	public double Location { get; }
	public double Scale { get; }

	public override double Calculate()
	{
		var u = RandomUtils.NextDouble();

		return Location + Scale * Math.Tan(Math.PI * (u - 0.5));
	}

	public override double GetExpectedValue()
	{
		return double.NaN;
	}

	public override double GetVariance()
	{
		return double.NaN;
	}

	public override double GetMinValue()
	{
		return double.NegativeInfinity;
	}

	public override double GetMaxValue()
	{
		return double.PositiveInfinity;
	}

	public override string ToString()
	{
		return $"Cauchy [Location = {Location:F3}, Scale = {Scale:F3}]";
	}
}