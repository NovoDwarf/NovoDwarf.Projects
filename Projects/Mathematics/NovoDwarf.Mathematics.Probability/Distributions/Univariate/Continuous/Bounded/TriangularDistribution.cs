using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Bounded;

[Categories("Distributions", "Univariate", "Continious", "Bounded")]
public partial class TriangularDistribution : Distribution
{
	public override double Expected => (_minimum + _maximum + _mode) / 3.0;

	public override double Mean => (_minimum + _maximum + _mode) / 3.0;

	public override double Median => GetMedian();

	public override double Mode => _mode;

	public override double Variance => (Math.Pow(_minimum, 2) + Math.Pow(_maximum, 2) + Math.Pow(_mode, 2) - _minimum * _maximum - _minimum * _mode - _maximum * _mode) / 18.0;

	public override double Skewness => GetSkewness();

	public override double Kurtosis => -0.6;
	
	public override double StandardDeviation => Math.Sqrt(Variance);

	public override double Minimum => _minimum;

	public override double Maximum => _maximum;
	
	public double LeftSlope => 2 / ((_maximum - _minimum) * (_mode - _minimum));
	
	public double RightSlope => 2 / ((_maximum - _minimum) * (_maximum - _mode));

	[EntityParameter(typeof(double), nameof(Minimum))]
	private double _minimum = 0;
	
	[EntityParameter(typeof(double), nameof(Maximum))]
	private double _maximum = 10;
	
	[EntityParameter(typeof(double), nameof(Mode))]
	private double _mode = 5;

	protected override void Validate()
	{
		if (_minimum >= _maximum)
			throw new ArgumentOutOfRangeException(nameof(_minimum), "Min must be less than max");
		
		if (_mode < _minimum || _mode > _maximum)
			throw new ArgumentOutOfRangeException(nameof(_mode), "Mode must be between min and max");
	}

	public override double Distribute()
	{
		var u = RandomUtils.NextDouble();
		var fc = (_mode - _minimum) / (_maximum - _minimum);

		return u < fc
			? _minimum + Math.Sqrt(u * (_maximum - _minimum) * (_mode - _minimum))
			: _maximum - Math.Sqrt((1 - u) * (_maximum - _minimum) * (_maximum - _mode));
	}

	public override double Quantile(double p)
	{
		return p; // TODO: impelement this
	}
	
	public override double ProbabilityDensity(double x)
	{
		if (x < _minimum || x > _maximum)
			return 0;

		if (x < _mode)
			return 2 * (x - _minimum) / ((_maximum - _minimum) * (_mode - _minimum));

		if (x > _mode)
			return 2 * (_maximum - x) / ((_maximum - _minimum) * (_maximum - _mode));
		
		return 2 / (_maximum - _minimum);
	}

	public override double CumulativeDistribution(double x)
	{
		if (x < _minimum)
			return 0;
		
		if (x > _maximum)
			return 1;

		if (x <= _mode)
			return Math.Pow(x - _minimum, 2) / ((_maximum - _minimum) * (_mode - _minimum));

		return 1 - Math.Pow(_maximum - x, 2) / ((_maximum - _minimum) * (_maximum - _mode));
	}
	
	public override string ToString() => $"Triangular Distribution [Min = {_minimum}, Max = {_maximum}, Mode = {_mode}]";

	public override bool Equals(object? obj)
	{
		return obj is TriangularDistribution other 
		       && DoubleUtils.Approximately(_minimum, other._minimum) 
		       && DoubleUtils.Approximately(_maximum, other._maximum) 
			 && DoubleUtils.Approximately(_mode, other._mode);
	}

	public override int GetHashCode() => HashCode.Combine(_minimum, _maximum, _mode);
	
	private double GetMedian()
	{
		var mid = (_minimum + _maximum) / 2.0;
		
		if (_mode >= mid)
			return _minimum + Math.Sqrt((_maximum - _minimum) * (_mode - _minimum) / 2.0);

		return _maximum - Math.Sqrt((_maximum - _minimum) * (_maximum - _mode) / 2.0);
	}
	
	private double GetSkewness()
	{
		var numerator = Math.Sqrt(2) * (_minimum + _maximum - 2 * _mode) * (2 * _minimum - _maximum - _mode) * (_minimum - 2 * _maximum + _mode);
		var denominator = 5 * Math.Pow(_minimum * _minimum + _maximum * _maximum + _mode * _mode - _minimum * _maximum - _minimum * _mode - _maximum * _mode, 1.5);
		
		return numerator / denominator;
	}
}