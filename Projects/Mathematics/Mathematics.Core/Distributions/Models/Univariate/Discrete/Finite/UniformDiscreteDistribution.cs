using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Discrete.Finite;

public partial class UniformDiscreteDistribution : Distribution
{
	private readonly int _min;
	private readonly int _max;
	
	public UniformDiscreteDistribution(int min, int max)
	{
		if (min >= max)
			throw new ArgumentOutOfRangeException(nameof(min), "Minimum value must be less than maximum value.");

		_min = min;
		_max = max;
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
		return RandomUtils.Next(_min, _max + 1);
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