using Mathematics.Distributions.Base;

namespace Mathematics.Distributions.Models.Degenerate;

public class DegenerateDistribution : DistributionBase
{
	public DegenerateDistribution(double constant)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(constant);
		
		Constant = constant;
	}

	public double Constant { get; }

	public override double Calculate() => Constant;

	public override double GetExpectedValue() => Constant;

	public override double GetVariance() => 0;

	public override double GetMinValue() => Constant;

	public override double GetMaxValue() => Constant;

	public override string ToString() => $"Degenerate [Constant = {Constant:F3}]";
}