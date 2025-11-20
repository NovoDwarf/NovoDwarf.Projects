using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class ExpoDistribution : Distribution
{
	public ExpoDistribution(double rates)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(rates, 0);

		Rates = rates;
	}

	public double Rates { get; }

	public override double Calculate()
	{
		var u = RandomUtils.NextDoubleSafe();

		return -Math.Log(u) / Rates;
	}

	public override double GetProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetCumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetExpectedValue() => 1.0 / Rates;
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

	public override double GetVariance() => 1.0 / (Rates * Rates);
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

	public override double GetMinValue() => 0;

	public override double GetMaxValue() => double.PositiveInfinity;

	public override string ToString() => $"Exponential [Lambda = {Rates:F3}]";
}