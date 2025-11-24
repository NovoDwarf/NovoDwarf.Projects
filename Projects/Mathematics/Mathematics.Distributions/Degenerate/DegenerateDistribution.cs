using Mathematics.Core.Base;

namespace Mathematics.Distributions.Degenerate;

public partial class DegenerateDistribution : Distribution
{
	private readonly double _constant;
	
	public DegenerateDistribution(double constant = 1)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(constant);

		_constant = constant;
	}
	
	public override double Expected => _constant;

	public override double Mean => _constant;

	public override double Median => _constant;

	public override double Mode => _constant;

	public override double Variance => 0;

	public override double Skewness => double.NaN;

	public override double Kurtosis => double.NaN;

	public override double StandardDeviation => 0;

	public override double Minimum => _constant;

	public override double Maximum => _constant;
	
	public override double Distribute() => _constant;

	public override double Quantile(double p)
	{
		if (p is <= 0 or >= 1)
			throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");
		
		return _constant;
	}

	public override double ProbabilityDensity(double x)
	{
		return x == _constant
			? double.PositiveInfinity
			: 0;
	}

	public override double CumulativeDistribution(double x) => x < _constant ? 0 : 1;

	public override string ToString() => $"Degenerate Distribution [Constant = {_constant}]";

	public override bool Equals(object? obj) => obj is DegenerateDistribution other && _constant == other._constant;

	public override int GetHashCode() => _constant.GetHashCode();
}