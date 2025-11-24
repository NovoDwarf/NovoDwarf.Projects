using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Continuous.Bounded;

public partial class UniformDistribution : Distribution
{
	private readonly double _min;
	private readonly double _max;
	
	public UniformDistribution(double min = 0, double max = 1)
	{
		if (min >= max)
			throw new ArgumentOutOfRangeException(nameof(min), "Min must be less than max");

		_min = min;
		_max = max;
	}

	public override double Expected => (_min + _max) / 2.0;

	public override double Mean => (_min + _max) / 2.0;

	public override double Median => (_min + _max) / 2.0;

	public override double Mode => double.NaN;

	public override double Variance => Math.Pow(_max - _min, 2) / 12.0;

	public override double Skewness => 0;

	public override double Kurtosis => -6.0 / 5.0;

	public override double StandardDeviation => Math.Sqrt(Variance);

	public override double Minimum => _min;

	public override double Maximum => _max;
	
	public override double Distribute() => RandomUtils.NextDouble(_min, _max);

	public override double Quantile(double p)
	{
		return p; // TODO: impelement this
	}

	public override double ProbabilityDensity(double x) => x < _min || x > _max ? 0 : 1.0 / (_max - _min);

	public override double CumulativeDistribution(double x)
	{
		if (x < _min)
			return 0;
		
		if (x > _max)
			return 1;
		
		return (x - _min) / (_max - _min);
	}
	
	public override string ToString() => $"Uniform Distribution [Min = {_min}, Max = {_max}]";

	public override bool Equals(object? obj) => obj is UniformDistribution other && _min == other._min && _max == other._max;

	public override int GetHashCode() => HashCode.Combine(_min, _max);
}