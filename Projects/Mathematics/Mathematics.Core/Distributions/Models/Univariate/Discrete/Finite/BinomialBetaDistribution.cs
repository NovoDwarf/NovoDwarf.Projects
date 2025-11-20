using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Distributions.Models.Univariate.Continuous.Bounded;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Discrete.Finite;

public partial class BinomialBetaDistribution : Distribution
{
	private readonly BetaDistribution _betaDist;
	
	private readonly double _alpha;
	private readonly double _beta;
	private readonly int _trials;
	
	public BinomialBetaDistribution(double alpha, double beta, int trials)
	{
		if (alpha <= 0)
			throw new ArgumentOutOfRangeException(nameof(alpha), "Alpha must be positive.");
		if (beta <= 0)
			throw new ArgumentOutOfRangeException(nameof(beta), "Beta must be positive.");
		if (trials < 0)
			throw new ArgumentOutOfRangeException(nameof(trials), "Number of trials must be non-negative.");

		_alpha = alpha;
		_beta = beta;
		_trials = trials;
		_betaDist = new BetaDistribution(alpha, beta, 1);
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
		var p = _betaDist.Distribute();

		return SampleBinomial(_trials, p);
	}

	public override double ProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double CumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	private int SampleBinomial(int n, double p)
	{
		if (n == 0 || p == 0)
			return 0;

		if (p == 1)
			return n;

		var successes = 0;

		for (var i = 0; i < n; i++)
			if (RandomUtils.NextDouble() < p)
				successes++;
		return successes;
	}
}