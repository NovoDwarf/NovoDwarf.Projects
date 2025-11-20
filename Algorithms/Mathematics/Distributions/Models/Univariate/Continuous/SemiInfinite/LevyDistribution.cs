using Mathematics.Distributions.Base;
using Mathematics.Transforms.Models;

namespace Mathematics.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class LevyDistribution : Distribution
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

	public override double GetProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetCumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetExpectedValue()
	{
		return double.PositiveInfinity;
	}

	public override double GetMean()
	{
		throw new NotImplementedException();
	}

	public override double GetMedian()
	{
		throw new NotImplementedException();
	}

	public override double GetMode()
	{
		throw new NotImplementedException();
	}

	public override double GetVariance()
	{
		return double.PositiveInfinity;
	}

	public override double GetSkewness()
	{
		throw new NotImplementedException();
	}

	public override double GetKurtosis()
	{
		throw new NotImplementedException();
	}

	public override double GetStandardDeviation()
	{
		throw new NotImplementedException();
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