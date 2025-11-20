using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.Bounded;

public partial class TriangularDistribution : Distribution
{
	private readonly double _min;
	private readonly double _max;
	private readonly double _mode;
	
	public TriangularDistribution(double min, double max, double mode)
	{
		_min = min;
		_max = max;
		_mode = mode;
	}
	
	public override double Expected => (_min + _max + _mode) / 3.0;

	public override double Mean => throw new NotImplementedException();

	public override double Median => throw new NotImplementedException();

	public override double Mode => throw new NotImplementedException();

	public override double Variance => (Math.Pow(_min, 2) + Math.Pow(_max, 2) + Math.Pow(_mode, 2) - _min * _max - _min * _mode - _max * _mode) / 18.0;

	public override double Skewness => throw new NotImplementedException();

	public override double Kurtosis => throw new NotImplementedException();

	public override double StandardDeviation => throw new NotImplementedException();

	public override double Minimum => _min;

	public override double Maximum => _max;
	
	public override double Distribute()
	{
		var u = RandomUtils.NextDouble();
		var fc = (_mode - _min) / (_max - _min);

		return u < fc
			? _min + Math.Sqrt(u * (_max - _min) * (_mode - _min))
			: _max - Math.Sqrt((1 - u) * (_max - _min) * (_max - _mode));
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