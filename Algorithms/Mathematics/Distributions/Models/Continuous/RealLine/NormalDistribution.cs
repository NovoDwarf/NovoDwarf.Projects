using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Continuous.RealLine;

public class NormalDistribution(double mean, double stdDev) : DistributionBase
{
	public double Mean { get; } = mean;
	public double StandardDeviation { get; } = stdDev;

	public override double Calculate() => RandomUtils.NextNormal();

	public override double GetExpectedValue() => Mean;

	public override double GetVariance() => StandardDeviation * StandardDeviation;

	public override double GetMinValue() => double.NegativeInfinity;

	public override double GetMaxValue() => double.PositiveInfinity;

	public override string ToString() => $"Normal [Mean = {Mean:F3}, Standard Deviation = {StandardDeviation:F3}]";
}