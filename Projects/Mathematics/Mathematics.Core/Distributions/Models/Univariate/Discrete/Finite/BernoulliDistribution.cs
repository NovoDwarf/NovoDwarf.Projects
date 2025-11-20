using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Discrete.Finite;

public partial class BernoulliDistribution : Distribution
{
	private readonly double _probability;
	
	public BernoulliDistribution(double probability)
	{
		if (probability is < 0 or > 1)
			throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1.");

		_probability = probability;
	}

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
		return RandomUtils.NextDouble() < _probability ? 1.0 : 0.0;
	}

	public override double ProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double CumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}
}