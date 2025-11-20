using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;
using Utilities.Extensions;

namespace Mathematics.Core.Distributions.Models.Univariate.Discrete.Finite;

public partial class BinomialPoissonDistribution : Distribution
{
	private readonly double[] _success;

	public BinomialPoissonDistribution(double[] success)
	{
		ArgumentException.ThrowIfNullOrEmpty(success);
		ArgumentOutOfRangeException.ThrowIfOutOfRange(success, 0, 1);

		_success = (double[])success.Clone();
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
		return SamplePoissonBinomial(_success);
	}

	public override double ProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double CumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	private int SamplePoissonBinomial(double[] probabilities)
	{
		if (probabilities.Length == 0)
			return 0;

		return probabilities.Count(p => RandomUtils.NextDouble() < p);
	}
}