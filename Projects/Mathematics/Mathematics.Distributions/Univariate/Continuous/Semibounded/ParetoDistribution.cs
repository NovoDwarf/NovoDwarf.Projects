using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Continuous.Semibounded;

public partial class ParetoDistribution : Distribution
{
	private readonly double _scale;
	private readonly double _shape;
	
	public ParetoDistribution(double scale = 1, double shape = 1)
	{
		if (scale <= 0)
			throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be greater than 0");
		
		if (shape <= 0)
			throw new ArgumentOutOfRangeException(nameof(shape), "Shape must be greater than 0");

		_scale = scale;
		_shape = shape;
	}

	public override double Expected => _shape > 1 ? _shape * _scale / (_shape - 1) : double.PositiveInfinity;
	
	public override double Mean => _shape > 1 ? _shape * _scale / (_shape - 1) : double.PositiveInfinity;
	
	public override double Median => _scale * Math.Pow(2, 1 / _shape);
	
	public override double Mode => _scale;
	
	public override double Variance => GetVarience();

	public override double Skewness => GetSkewness();
	
	public override double Kurtosis => GetKurtosis();
	
	public override double StandardDeviation => GetStandardDeviation();
	
	public override double Minimum => _scale;
	
	public override double Maximum => double.PositiveInfinity;

	public double Scale => _scale;
	
	public double Shape => _shape;
	
	public override double Distribute()
	{
		var u = RandomUtils.NextDouble();
		
		return _scale / Math.Pow(u, 1.0 / _shape);
	}

	public override double Quantile(double p)
	{
		return p switch
		{
			< 0 or > 1 => throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1"),
			0 => _scale,
			1 => double.PositiveInfinity,
			_ => _scale / Math.Pow(1 - p, 1 / _shape)
		};
	}
	
	public override double ProbabilityDensity(double x)
	{
		return x < _scale 
			? 0 
			: _shape * Math.Pow(_scale, _shape) / Math.Pow(x, _shape + 1);
	}

	public override double CumulativeDistribution(double x)
	{
		return x < _scale 
			? 0 
			: 1 - Math.Pow(_scale / x, _shape);
	}
	
	public override string ToString() => $"Pareto Distribution [Scale = {_scale}, Shape = {_shape}]";

	public override bool Equals(object? obj) => obj is ParetoDistribution other && _scale == other._scale && _shape == other._shape;

	public override int GetHashCode() => HashCode.Combine(_scale, _shape);

	private double GetVarience()
	{
		return _shape switch
		{
			> 2 => _scale * _scale * _shape / (Math.Pow(_shape - 1, 2) * (_shape - 2)),
			> 1 => double.PositiveInfinity,
			_ => double.NaN
		};
	}
	
	private double GetSkewness()
	{
		return _shape > 3
			? 2 * (1 + _shape) / (_shape - 3) * Math.Sqrt((_shape - 2) / _shape)
			: double.PositiveInfinity;
	}
	
	private double GetKurtosis()
	{
		return _shape > 4
			? 6 * (Math.Pow(_shape, 3) + Math.Pow(_shape, 2) - 6 * _shape - 2) / (_shape * (_shape - 3) * (_shape - 4))
			: double.PositiveInfinity;
	}
	
	private double GetStandardDeviation()
	{
		var variance = Variance;
		
		return double.IsFinite(variance) ? Math.Sqrt(variance) : double.PositiveInfinity;
	}
}