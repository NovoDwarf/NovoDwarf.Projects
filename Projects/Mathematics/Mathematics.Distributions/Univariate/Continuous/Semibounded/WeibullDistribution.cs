using Mathematics.Core.Base;
using Mathematics.Core.Utilities;
using Mathematics.Functions;

namespace Mathematics.Distributions.Univariate.Continuous.Semibounded;

public partial class WeibullDistribution : Distribution
{
	private readonly double _scale;
	private readonly double _shape;
	
	public WeibullDistribution(double scale = 1, double shape = 1)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(scale, 0);
		ArgumentOutOfRangeException.ThrowIfLessThan(shape, 0);

		_scale = scale;
		_shape = shape;
	}
	
	public override double Expected => _scale * GammaFunction.Calculate(1 + 1 / _shape);
	
	public override double Mean => _scale * GammaFunction.Calculate(1 + 1 / _shape);
	
	public override double Median => _scale * Math.Pow(Math.Log(2), 1 / _shape);
	
	public override double Mode => _shape <= 1 ? 0 : _scale * Math.Pow((_shape - 1) / _shape, 1 / _shape);

	public override double Variance => GetVariance();
	
	public override double Skewness => GetSkewness();
	
	public override double Kurtosis => GetKurtosis();
	
	public override double StandardDeviation => Math.Sqrt(Variance);
	
	public override double Minimum => 0;
	
	public override double Maximum => double.PositiveInfinity;

	public double Scale => _scale;
	
	public double Shape => _shape;
	
	public override double Distribute()
	{
		var u = RandomUtils.NextDoubleSafe();
		
		return _scale * Math.Pow(-Math.Log(u), 1.0 / _shape);
	}

	public override double Quantile(double p)
	{
		return p switch
		{
			< 0 or > 1 => throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1"),
			0 => 0,
			1 => double.PositiveInfinity,
			_ => _scale * Math.Pow(-Math.Log(1 - p), 1.0 / _shape)
		};
	}
	
	public override double ProbabilityDensity(double x)
	{
		switch (x)
		{
			case < 0:
				return 0;
			case 0 when _shape < 1:
				return double.PositiveInfinity;
			case 0 when _shape == 1:
				return 1.0 / _scale;
			case 0:
				return 0;
		}

		var term1 = _shape / _scale;
		var term2 = Math.Pow(x / _scale, _shape - 1);
		var term3 = Math.Exp(-Math.Pow(x / _scale, _shape));
		
		return term1 * term2 * term3;
	}

	public override double CumulativeDistribution(double x)
	{
		if (x < 0)
			return 0;

		return 1 - Math.Exp(-Math.Pow(x / _scale, _shape));
	}
	
	public override string ToString() => $"Weibull Distribution [Scale = {_scale}, Shape = {_shape}]";

	public override bool Equals(object obj) => obj is WeibullDistribution other && _scale == other._scale && _shape == other._shape;

	public override int GetHashCode() => HashCode.Combine(_scale, _shape);
	
	private double GetVariance()
	{
		var gamma1 = GammaFunction.Calculate(1 + 1 / _shape);
		var gamma2 = GammaFunction.Calculate(1 + 2 / _shape);
			
		return _scale * _scale * (gamma2 - gamma1 * gamma1);
	}
	
	private double GetKurtosis()
	{
		var gamma1 = GammaFunction.Calculate(1 + 1 / _shape);
		var gamma2 = GammaFunction.Calculate(1 + 2 / _shape);
		var gamma3 = GammaFunction.Calculate(1 + 3 / _shape);
		var gamma4 = GammaFunction.Calculate(1 + 4 / _shape);
		
		var variance = Variance;
		var mean = Mean;
			
		var term1 = gamma4;
		var term2 = -4 * gamma3 * gamma1;
		var term3 = 6 * gamma2 * gamma1 * gamma1;
		var term4 = -3 * gamma1 * gamma1 * gamma1 * gamma1;
			
		return (term1 + term2 + term3 + term4) / (variance * variance) - 3;
	}

	private double GetSkewness()
	{
		var gamma1 = GammaFunction.Calculate(1 + 1 / _shape);
		var gamma2 = GammaFunction.Calculate(1 + 2 / _shape);
		var gamma3 = GammaFunction.Calculate(1 + 3 / _shape);
		var variance = Variance;
			
		var numerator = gamma3 - 3 * gamma2 * gamma1 + 2 * gamma1 * gamma1 * gamma1;
		var denominator = Math.Pow(variance, 1.5);
			
		return numerator / denominator;
	}
}