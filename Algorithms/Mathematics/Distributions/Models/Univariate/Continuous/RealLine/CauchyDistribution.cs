using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Continuous.RealLine;

public partial class CauchyDistribution : Distribution
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
		return double.NaN;
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
		return double.NaN;
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