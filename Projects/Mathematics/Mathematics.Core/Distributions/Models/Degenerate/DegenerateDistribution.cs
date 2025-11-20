using Mathematics.Core.Distributions.Base;

namespace Mathematics.Core.Distributions.Models.Degenerate;

public partial class DegenerateDistribution : Distribution
{
	private readonly double _constant;
	
	public DegenerateDistribution(double constant)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(constant);

		_constant = constant;
	}
	
	public override double Expected => _constant;

	public override double Mean => throw new NotImplementedException();

	public override double Median => throw new NotImplementedException();

	public override double Mode => throw new NotImplementedException();

	public override double Variance=> 0;

	public override double Skewness => throw new NotImplementedException();

	public override double Kurtosis => throw new NotImplementedException();

	public override double StandardDeviation => throw new NotImplementedException();

	public override double Minimum => _constant;

	public override double Maximum => _constant;
	
	public override double Distribute() => _constant;

	public override double ProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double CumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}
}