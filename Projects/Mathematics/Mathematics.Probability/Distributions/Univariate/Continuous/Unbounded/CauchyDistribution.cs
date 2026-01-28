using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Unbounded;

[Categories("Distributions", "Univariate", "Continious", "Unbounded")]
public partial class CauchyDistribution : Distribution
{
	public override double Expected => double.NaN;

	public override double Mean => double.NaN;

	public override double Median => Location;

	public override double Mode => Location;

	public override double Variance => double.NaN;

	public override double Skewness => double.NaN;

	public override double Kurtosis => double.NaN;

	public override double StandardDeviation => double.NaN;

	public override double Minimum => double.NegativeInfinity;

	public override double Maximum => double.PositiveInfinity;

	[EntityParameter(typeof(double), nameof(Location))]
	public double Location { get; private set; } = 0;
	
	[EntityParameter(typeof(double), nameof(Scale))]
	public double Scale { get; private set; } = 1;

	protected override void Validate()
	{
		if (Scale <= 0)
			throw new ArgumentOutOfRangeException(nameof(Scale), "Scale must be positive");
	}

	public override double Distribute()
	{
		var u = RandomUtils.NextDouble();
		
		return Location + Scale * Math.Tan(Math.PI * (u - 0.5));
	}

	public override double ProbabilityDensity(double x) => 1.0 / (Math.PI * Scale * (1 + Math.Pow((x - Location) / Scale, 2)));

	public override double CumulativeDistribution(double x) => 1.0 / Math.PI * Math.Atan((x - Location) / Scale) + 0.5;

	public override double Quantile(double p)
	{
		if (p is <= 0 or >= 1)
			throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

		return Location + Scale * Math.Tan(Math.PI * (p - 0.5));
	}
	
	public override string ToString() => $"Cauchy Distribution [Location = {Location}, Scale = {Scale}]";

	public override bool Equals(object? obj) => obj is CauchyDistribution other && DoubleUtils.Approximately(Location, other.Location) && DoubleUtils.Approximately(Scale, other.Scale);

	public override int GetHashCode() => HashCode.Combine(Location, Scale);
}