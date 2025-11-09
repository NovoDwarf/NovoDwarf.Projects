using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Continuous.Bounded;

public class UniformDistribution : DistributionBase
{
	public UniformDistribution(double min, double max)
	{
		Min = min;
		Max = max;
	}

	public double Min { get; }
	public double Max { get; }

	/// <inheritdoc cref="DistributionBase.Calculate()"/>
	public override double Calculate() => RandomUtils.NextDouble(Min, Max);

	/// <inheritdoc cref="DistributionBase.GetExpectedValue()"/>
	public override double GetExpectedValue() => (Min + Max) / 2.0;

	/// <inheritdoc cref="DistributionBase.GetVariance()"/>
	public override double GetVariance() => Math.Pow(Max - Min, 2) / 12.0;

	/// <inheritdoc cref="DistributionBase.GetMinValue()"/>
	public override double GetMinValue() => Min;

	/// <inheritdoc cref="DistributionBase.GetMaxValue()"/>
	public override double GetMaxValue() => Max;

	/// <inheritdoc cref="DistributionBase.ToString()"/>
	public override string ToString() => $"Uniform [Min = {Min:F3}, Max = {Max:F3})";
}