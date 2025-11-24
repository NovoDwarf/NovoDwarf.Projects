using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Continuous.Bounded;

public partial class TriangularDistribution : Distribution
{
	private readonly double _min;
	private readonly double _max;
	private readonly double _mode;
	
	public TriangularDistribution(double min = 0, double max = 10, double mode = 5)
	{
		if (min >= max)
			throw new ArgumentOutOfRangeException(nameof(min), "Min must be less than max");
		if (mode < min || mode > max)
			throw new ArgumentOutOfRangeException(nameof(mode), "Mode must be between min and max");

		_min = min;
		_max = max;
		_mode = mode;
	}
	
	public override double Expected => (_min + _max + _mode) / 3.0;

	public override double Mean => (_min + _max + _mode) / 3.0;

	public override double Median => GetMedian();

	public override double Mode => _mode;

	public override double Variance => (Math.Pow(_min, 2) + Math.Pow(_max, 2) + Math.Pow(_mode, 2) - _min * _max - _min * _mode - _max * _mode) / 18.0;

	public override double Skewness => GetSkewness();

	public override double Kurtosis => -0.6;
	
	public override double StandardDeviation => Math.Sqrt(Variance);

	public override double Minimum => _min;

	public override double Maximum => _max;
	
	public double LeftSlope => 2 / ((_max - _min) * (_mode - _min));
	
	public double RightSlope => 2 / ((_max - _min) * (_max - _mode)); 
	
	public override double Distribute()
	{
		var u = RandomUtils.NextDouble();
		var fc = (_mode - _min) / (_max - _min);

		return u < fc
			? _min + Math.Sqrt(u * (_max - _min) * (_mode - _min))
			: _max - Math.Sqrt((1 - u) * (_max - _min) * (_max - _mode));
	}

	public override double Quantile(double p)
	{
		return p; // TODO: impelement this
	}
	
	public override double ProbabilityDensity(double x)
	{
		if (x < _min || x > _max)
			return 0;

		if (x < _mode)
			return 2 * (x - _min) / ((_max - _min) * (_mode - _min));

		if (x > _mode)
			return 2 * (_max - x) / ((_max - _min) * (_max - _mode));
		
		return 2 / (_max - _min);
	}

	public override double CumulativeDistribution(double x)
	{
		if (x < _min)
			return 0;
		
		if (x > _max)
			return 1;

		if (x <= _mode)
			return Math.Pow(x - _min, 2) / ((_max - _min) * (_mode - _min));

		return 1 - Math.Pow(_max - x, 2) / ((_max - _min) * (_max - _mode));
	}
	
	public override string ToString() => $"Triangular Distribution [Min = {_min}, Max = {_max}, Mode = {_mode}]";

	public override bool Equals(object? obj) => obj is TriangularDistribution other && _min == other._min && _max == other._max && _mode == other._mode;

	public override int GetHashCode() => HashCode.Combine(_min, _max, _mode);
	
	private double GetMedian()
	{
		var mid = (_min + _max) / 2.0;
		
		if (_mode >= mid)
			return _min + Math.Sqrt((_max - _min) * (_mode - _min) / 2.0);

		return _max - Math.Sqrt((_max - _min) * (_max - _mode) / 2.0);
	}
	
	private double GetSkewness()
	{
		var numerator = Math.Sqrt(2) * (_min + _max - 2 * _mode) * (2 * _min - _max - _mode) * (_min - 2 * _max + _mode);
		var denominator = 5 * Math.Pow(_min * _min + _max * _max + _mode * _mode - _min * _max - _min * _mode - _max * _mode, 1.5);
		
		return numerator / denominator;
	}
}