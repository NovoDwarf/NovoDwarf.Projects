using System.ComponentModel.DataAnnotations;
using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

[Categories("Distributions", "Univariate", "Continious", "Semibounded")]
public partial class ParetoDistribution : Distribution
{
	public override double Expected => Shape > 1 ? Shape * Scale / (Shape - 1) : double.PositiveInfinity;
	
	public override double Mean => Shape > 1 ? Shape * Scale / (Shape - 1) : double.PositiveInfinity;
	
	public override double Median => Scale * Math.Pow(2, 1 / Shape);
	
	public override double Mode => Scale;
	
	public override double Variance => GetVarience();

	public override double Skewness => GetSkewness();
	
	public override double Kurtosis => GetKurtosis();
	
	public override double StandardDeviation => GetStandardDeviation();
	
	public override double Minimum => Scale;
	
	public override double Maximum => double.PositiveInfinity;

	[EntityParameter(typeof(double), nameof(Shape))]
	[Range(0, double.PositiveInfinity)]
	public double Shape { get; private set; } = 1;
	
	[EntityParameter(typeof(double), nameof(Scale))]
	[Range(0, double.PositiveInfinity)]
	public double Scale { get; private set; } = 1;
	
	protected override void Validate()
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(Scale, 0);
		ArgumentOutOfRangeException.ThrowIfLessThan(Shape, 0);
	}

	public override double Distribute()
	{
		var u = RandomUtils.NextDouble();
		
		return Scale / Math.Pow(u, 1.0 / Shape);
	}

	public override double Quantile(double p)
	{
		return p switch
		{
			< 0 or > 1 => throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1"),
			0 => Scale,
			1 => double.PositiveInfinity,
			_ => Scale / Math.Pow(1 - p, 1 / Shape)
		};
	}
	
	public override double ProbabilityDensity(double x)
	{
		return x < Scale 
			? 0 
			: Shape * Math.Pow(Scale, Shape) / Math.Pow(x, Shape + 1);
	}

	public override double CumulativeDistribution(double x)
	{
		return x < Scale 
			? 0 
			: 1 - Math.Pow(Scale / x, Shape);
	}
	
	public override string ToString() => $"Pareto Distribution [Scale = {Scale}, Shape = {Shape}]";

	public override bool Equals(object? obj) => obj is ParetoDistribution other && Scale == other.Scale && Shape == other.Shape;

	public override int GetHashCode() => HashCode.Combine(Scale, Shape);

	private double GetVarience()
	{
		return Shape switch
		{
			> 2 => Scale * Scale * Shape / (Math.Pow(Shape - 1, 2) * (Shape - 2)),
			> 1 => double.PositiveInfinity,
			_ => double.NaN
		};
	}
	
	private double GetSkewness()
	{
		return Shape > 3
			? 2 * (1 + Shape) / (Shape - 3) * Math.Sqrt((Shape - 2) / Shape)
			: double.PositiveInfinity;
	}
	
	private double GetKurtosis()
	{
		return Shape > 4
			? 6 * (Math.Pow(Shape, 3) + Math.Pow(Shape, 2) - 6 * Shape - 2) / (Shape * (Shape - 3) * (Shape - 4))
			: double.PositiveInfinity;
	}
	
	private double GetStandardDeviation()
	{
		var variance = Variance;
		
		return double.IsFinite(variance) ? Math.Sqrt(variance) : double.PositiveInfinity;
	}
}