using System.ComponentModel.DataAnnotations;
using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;
using Mathematics.Numerical.Simple;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

[Categories("Distributions", "Univariate", "Continious", "Semibounded")]
public partial class WeibullDistribution : Distribution
{
	public override double Expected => Scale * GammaFunction.Calculate(1 + 1 / Shape);
	
	public override double Mean => Scale * GammaFunction.Calculate(1 + 1 / Shape);
	
	public override double Median => Scale * Math.Pow(Math.Log(2), 1 / Shape);
	
	public override double Mode => Shape <= 1 ? 0 : Scale * Math.Pow((Shape - 1) / Shape, 1 / Shape);

	public override double Variance => GetVariance();
	
	public override double Skewness => GetSkewness();
	
	public override double Kurtosis => GetKurtosis();
	
	public override double StandardDeviation => Math.Sqrt(Variance);
	
	public override double Minimum => 0;
	
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
		var u = RandomUtils.NextDoubleSafe();
		
		return Scale * Math.Pow(-Math.Log(u), 1.0 / Shape);
	}

	public override double Quantile(double p)
	{
		return p switch
		{
			< 0 or > 1 => throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1"),
			0 => 0,
			1 => double.PositiveInfinity,
			_ => Scale * Math.Pow(-Math.Log(1 - p), 1.0 / Shape)
		};
	}
	
	public override double ProbabilityDensity(double x)
	{
		switch (x)
		{
			case < 0:
				return 0;
			case 0 when Shape < 1:
				return double.PositiveInfinity;
			case 0 when Shape == 1:
				return 1.0 / Scale;
			case 0:
				return 0;
		}

		var term1 = Shape / Scale;
		var term2 = Math.Pow(x / Scale, Shape - 1);
		var term3 = Math.Exp(-Math.Pow(x / Scale, Shape));
		
		return term1 * term2 * term3;
	}

	public override double CumulativeDistribution(double x)
	{
		if (x < 0)
			return 0;

		return 1 - Math.Exp(-Math.Pow(x / Scale, Shape));
	}
	
	public override string ToString() => $"Weibull Distribution [Scale = {Scale}, Shape = {Shape}]";

	public override bool Equals(object? obj) => obj is WeibullDistribution other && Scale == other.Scale && Shape == other.Shape;

	public override int GetHashCode() => HashCode.Combine(Scale, Shape);
	
	private double GetVariance()
	{
		var gamma1 = GammaFunction.Calculate(1 + 1 / Shape);
		var gamma2 = GammaFunction.Calculate(1 + 2 / Shape);
			
		return Scale * Scale * (gamma2 - gamma1 * gamma1);
	}
	
	private double GetKurtosis()
	{
		var gamma1 = GammaFunction.Calculate(1 + 1 / Shape);
		var gamma2 = GammaFunction.Calculate(1 + 2 / Shape);
		var gamma3 = GammaFunction.Calculate(1 + 3 / Shape);
		var gamma4 = GammaFunction.Calculate(1 + 4 / Shape);
		
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
		var gamma1 = GammaFunction.Calculate(1 + 1 / Shape);
		var gamma2 = GammaFunction.Calculate(1 + 2 / Shape);
		var gamma3 = GammaFunction.Calculate(1 + 3 / Shape);
		var variance = Variance;
			
		var numerator = gamma3 - 3 * gamma2 * gamma1 + 2 * gamma1 * gamma1 * gamma1;
		var denominator = Math.Pow(variance, 1.5);
			
		return numerator / denominator;
	}
}