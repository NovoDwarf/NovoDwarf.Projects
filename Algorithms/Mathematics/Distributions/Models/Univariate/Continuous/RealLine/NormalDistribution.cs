using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Continuous.RealLine;

public partial class NormalDistribution(double mean, double stdDev) : Distribution
{
	public double Mean { get; } = mean;
	public double StandardDeviation { get; } = stdDev;

	public override double Calculate() => RandomUtils.NextNormal();
	public override double GetProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetCumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetExpectedValue() => Mean;
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

	public override double GetVariance() => StandardDeviation * StandardDeviation;
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

	public override double GetMinValue() => double.NegativeInfinity;

	public override double GetMaxValue() => double.PositiveInfinity;

	public override string ToString() => $"Normal [Mean = {Mean:F3}, Standard Deviation = {StandardDeviation:F3}]";
}