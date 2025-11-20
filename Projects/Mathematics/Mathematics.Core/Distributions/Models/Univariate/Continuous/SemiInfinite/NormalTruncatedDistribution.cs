using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Transforms.Models;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class NormalTruncatedDistribution : Distribution
{
	private readonly double _mean;
	private readonly double _standardDeviation;
	private readonly double _min;
	private readonly double _max;
	
	public NormalTruncatedDistribution(double mean, double standardDeviation, double min, double max)
	{
		_mean = mean;
		_standardDeviation = standardDeviation;
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
		double x;
		do
		{
			var (z, _) = BoxMullerTransform.Polar();
			x = _mean + _standardDeviation * z;
		} while (x < _min || x > _max);

		return x;
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