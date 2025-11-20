using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class NormalLogDistribution : Distribution
{
	public NormalLogDistribution(double mean, double standardDeviation)
	{
		Mean = mean;
		StandardDeviation = standardDeviation;
	}

	public double Mean { get; }
	public double StandardDeviation { get; }

	public override double Calculate()
	{
		var u = RandomUtils.NextNormal();

		return Math.Exp(u);
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
		return Math.Exp(Mean + StandardDeviation * StandardDeviation / 2.0);
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
		return Math.Exp(2 * Mean + StandardDeviation * StandardDeviation) *
		       (Math.Exp(StandardDeviation * StandardDeviation) - 1);
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
		return 0;
	}

	public override double GetMaxValue()
	{
		return double.PositiveInfinity;
	}

	public override string ToString()
	{
		return $"LogNormal [Mean = {Mean:F3}, std={StandardDeviation:F3})";
	}
}