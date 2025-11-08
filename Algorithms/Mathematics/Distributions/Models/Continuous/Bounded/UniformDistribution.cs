using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Continuous.Bounded;

public class UniformDistribution(double min, double max) : DistributionBase
{
	public double Min { get; } = min;
	public double Max { get; } = max;

	public override double Calculate() => RandomUtils.NextDouble(Min, Max);

	public override double GetExpectedValue() => (Min + Max) / 2.0;

	public override double GetVariance() => Math.Pow(Max - Min, 2) / 12.0;

	public override double GetMinValue() => Min;

	public override double GetMaxValue() => Max;

	public override string ToString() => $"Uniform [Min = {Min:F3}, Max = {Max:F3})";
}