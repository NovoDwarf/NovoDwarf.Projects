using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Discrete.Finite;

public class UniformDiscreteDistribution : DistributionBase
{
	public UniformDiscreteDistribution(int min, int max)
	{
		if (min >= max)
			throw new ArgumentOutOfRangeException(nameof(min), "Minimum value must be less than maximum value.");

		Min = min;
		Max = max;
	}

	public int Min { get; }
	public int Max { get; }

	public override double Calculate()
	{
		return RandomUtils.Next(Min, Max + 1);
	}

	public override double GetExpectedValue() => (Min + Max) / 2.0;

	public override double GetVariance()
	{
		var n = Max - Min + 1;
		
		return (n * n - 1) / 12.0;
	}

	public override double GetMinValue() => Min;

	public override double GetMaxValue() => Max;
}