using System.ComponentModel.DataAnnotations;
using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Degenerate;

[Categories("Distributions", "Degenerate")]
public sealed partial class DegenerateDistribution : Distribution
{
	public override double Expected => Constant;

	public override double Mean => Constant;

	public override double Median => Constant;

	public override double Mode => Constant;

	public override double Variance => 0;

	public override double Skewness => double.NaN;

	public override double Kurtosis => double.NaN;

	public override double StandardDeviation => 0;

	public override double Minimum => Constant;

	public override double Maximum => Constant;
	
	[Range(0, double.MaxValue)]
	[EntityParameter(typeof(double), nameof(Constant))]
	public double Constant { get; private set; } = 1;
	
	public override double Distribute() => Constant;

	public override double Quantile(double p)
	{
		if (p is <= 0 or >= 1)
			throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");
		
		return Constant;
	}

	public override double ProbabilityDensity(double x) => DoubleUtils.Approximately(x, Constant) ? double.PositiveInfinity : 0;

	public override double CumulativeDistribution(double x) => x < Constant ? 0 : 1;

	public override string ToString() => $"Degenerate Distribution [Constant = {Constant}]";

	public override bool Equals(object? obj) => obj is DegenerateDistribution other && DoubleUtils.Approximately(Constant, other.Constant);

	public override int GetHashCode() => Id.GetHashCode();

	protected override void Validate()
	{
		ArgumentOutOfRangeException.ThrowIfNegative(Constant);
	}
}