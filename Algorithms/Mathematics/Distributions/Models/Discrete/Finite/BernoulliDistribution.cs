using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Discrete.Finite;

public class BernoulliDistribution : DistributionBase
{
	public BernoulliDistribution(double probability)
	{
		if (probability is < 0 or > 1)
			throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1.");

		Probability = probability;
	}

	public double Probability { get; }

	public override double Calculate() => RandomUtils.NextDouble() < Probability ? 1.0 : 0.0;

	public override double GetExpectedValue() => Probability;

	public override double GetVariance() => Probability * (1 - Probability);

	public override double GetMinValue() => 0.0;

	public override double GetMaxValue() => 1.0;
}