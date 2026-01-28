using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Bounded;

[Categories("Distributions", "Univariate", "Continious", "Bounded")]
public partial class UniformDistribution : Distribution
{
	public override double Expected => (_minimum + _maximum) / 2.0;

	public override double Mean => (_minimum + _maximum) / 2.0;

	public override double Median => (_minimum + _maximum) / 2.0;

	public override double Mode => double.NaN;

	public override double Variance => Math.Pow(_maximum - _minimum, 2) / 12.0;

	public override double Skewness => 0;

	public override double Kurtosis => -6.0 / 5.0;

	public override double StandardDeviation => Math.Sqrt(Variance);

	public override double Minimum => _minimum;

	public override double Maximum => _maximum;

	[EntityParameter(typeof(double), nameof(Minimum))]
	private double _minimum = 0;
	
	[EntityParameter(typeof(double), nameof(Maximum))]
	private double _maximum = 1;

	protected override void Validate()
	{
		if (_minimum >= _maximum)
			throw new ArgumentOutOfRangeException(nameof(Minimum), "Minimum must be less than maximum");
	}

	public override double Distribute() => RandomUtils.NextDouble(_minimum, _maximum);

	public override double Quantile(double p)
	{
		return p; // TODO: impelement this
	}

	public override double ProbabilityDensity(double x) => x < _minimum || x > _maximum ? 0 : 1.0 / (_maximum - _minimum);

	public override double CumulativeDistribution(double x)
	{
		if (x < _minimum)
			return 0;
		
		if (x > _maximum)
			return 1;
		
		return (x - _minimum) / (_maximum - _minimum);
	}
	
	public override string ToString() => $"Uniform Distribution [Min = {_minimum}, Max = {_maximum}]";

	public override bool Equals(object? obj) => obj is UniformDistribution other && _minimum == other._minimum && _maximum == other._maximum;

	public override int GetHashCode() => HashCode.Combine(_minimum, _maximum);
}