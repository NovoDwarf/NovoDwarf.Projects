using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Discrete.Finite;

public partial class BinomialDistribution : Distribution
{
	private readonly int _trials;
	private readonly double _probability;
	
	public BinomialDistribution(int trials, double probability)
	{
		if (trials <= 0)
			throw new ArgumentOutOfRangeException(nameof(trials), "Number of trials must be positive.");

		if (probability is < 0 or > 1)
			throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1.");

		_trials = trials;
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
		var successes = 0;

		for (var i = 0; i < _trials; i++)
			if (RandomUtils.NextDouble() < _probability)
				successes++;

		return successes;
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