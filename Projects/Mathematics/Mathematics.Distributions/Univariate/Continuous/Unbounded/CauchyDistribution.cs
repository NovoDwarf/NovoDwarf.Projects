using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Continuous.Unbounded;

public partial class CauchyDistribution : Distribution
{
	private readonly double _location;
	private readonly double _scale;
	
	public CauchyDistribution(double location = 0, double scale = 1)
	{
		if (scale <= 0)
			throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be positive");

		_location = location;
		_scale = scale;
	}
	
	public override double Expected => double.NaN;

	public override double Mean => double.NaN;

	public override double Median => _location;

	public override double Mode => _location;

	public override double Variance => double.NaN;

	public override double Skewness => double.NaN;

	public override double Kurtosis => double.NaN;

	public override double StandardDeviation => double.NaN;

	public override double Minimum => double.NegativeInfinity;

	public override double Maximum => double.PositiveInfinity;

	public override double Distribute()
	{
		var u = RandomUtils.NextDouble();
		
		return _location + _scale * Math.Tan(Math.PI * (u - 0.5));
	}

	public override double ProbabilityDensity(double x) => 1.0 / (Math.PI * _scale * (1 + Math.Pow((x - _location) / _scale, 2)));

	public override double CumulativeDistribution(double x) => 1.0 / Math.PI * Math.Atan((x - _location) / _scale) + 0.5;

	public override double Quantile(double p)
	{
		if (p is <= 0 or >= 1)
			throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

		return _location + _scale * Math.Tan(Math.PI * (p - 0.5));
	}
	
	public override string ToString() => $"Cauchy Distribution [Location = {_location}, Scale = {_scale}]";

	public override bool Equals(object? obj) => obj is CauchyDistribution other && _location == other._location && _scale == other._scale;

	public override int GetHashCode() => HashCode.Combine(_location, _scale);
}