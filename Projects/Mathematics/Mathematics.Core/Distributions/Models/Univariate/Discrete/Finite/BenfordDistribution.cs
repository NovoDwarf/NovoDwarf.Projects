using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Discrete.Finite;

public partial class BenfordDistribution : Distribution
{
	public override double Expected { get; }
	public override double Mean { get; }
	public override double Median { get; }
	public override double Mode { get; }
	public override double Variance { get; }
	public override double Skewness { get; }
	public override double Kurtosis { get; }
	public override double StandardDeviation { get; }
	public override double Minimum { get; }
	public override double Maximum { get; }

	public override double Distribute()
	{
		var randomValue = RandomUtils.NextDouble();
		var cumulativeProbability = 0.0;

		for (var digit = 1; digit <= 9; digit++)
		{
			cumulativeProbability += GetProbabilityForDigit(digit);

			if (randomValue <= cumulativeProbability)
				return digit;
		}

		return 9;
	}

	public override double ProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double CumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	private static double GetProbabilityForDigit(int digit)
	{
		if (digit is < 1 or > 9)
			throw new ArgumentOutOfRangeException(nameof(digit), "Digit must be between 1 and 9.");

		return Math.Log10(1.0 + 1.0 / digit);
	}
}