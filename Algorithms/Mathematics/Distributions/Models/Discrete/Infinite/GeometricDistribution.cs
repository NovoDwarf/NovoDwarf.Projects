using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Discrete.Infinite;

public class GeometricDistribution : DistributionBase
{
	public GeometricDistribution(double probability)
	{
		if (probability is <= 0 or > 1)
			throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1.");

		Probability = probability;
	}

	public double Probability { get; }

	public override double Calculate()
	{
		return Math.Floor(Math.Log(1 - RandomUtils.NextDouble()) / Math.Log(1 - Probability)) + 1;
	}

	public override double GetExpectedValue() => 1.0 / Probability;

	public override double GetVariance() => (1 - Probability) / (Probability * Probability);

	public override double GetMinValue() => 1.0;

	public override double GetMaxValue() => double.PositiveInfinity;
}