using System.ComponentModel.DataAnnotations;
using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;
using Mathematics.Numerical.Simple;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

[Categories("Distributions", "Univariate", "Continious", "Semibounded")]
public partial class ErlangDistribution : Distribution
{
	public override double Expected => Shape / Rate;
	
	public override double Mean => Shape / Rate;

	public override double Median => Shape == 1 ? Math.Log(2) / Rate : (Shape - 1.0 / 3.0) / Rate;
	
	public override double Mode => Shape >= 1 ? (Shape - 1) / Rate : 0;

	public override double Variance => Shape / (Rate * Rate);
	
	public override double Skewness => 2.0 / Math.Sqrt(Shape);
	
	public override double Kurtosis => 6.0 / Shape;
	
	public override double StandardDeviation => Math.Sqrt(Variance);
	
	public override double Minimum => 0;
	
	public override double Maximum => double.PositiveInfinity;

	[EntityParameter(typeof(double), nameof(Shape))]
	[Range(1, int.MaxValue)]
	public int Shape { get; private set; } = 2;

	[EntityParameter(typeof(double), nameof(Rate))]
	[Range(0, double.PositiveInfinity)]
	public double Rate { get; private set; } = 1;

	protected override void Validate()
	{
		if (Shape < 1)
			throw new ArgumentException("Shape must be greater than 0.", nameof(Shape));

		if (Rate <= 0)
			throw new ArgumentException("Rate must be greater than 0.", nameof(Rate));
	}

	public override double Distribute()
	{
		var sum = 0.0;
		var exp = new ExpoDistribution();

		exp.Set(Rate);
		
		for (var i = 0; i < Shape; i++)
			sum += exp.Distribute();

		return sum;
	}
	
	public override double Quantile(double p)
	{
		if (p < 0 || p > 1)
			throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

		switch (p)
		{
			case 0: return 0;
			case 1: return double.PositiveInfinity;
		}

		double low = 0;
		double high = 1;

		while (CumulativeDistribution(high) < p) 
			high *= 2;

		const double tolerance = 1e-10;
		const int maxIterations = 100;

		for (var i = 0; i < maxIterations; i++)
		{
			var mid = (low + high) / 2;
			var fmid = CumulativeDistribution(mid);

			if (Math.Abs(fmid - p) < tolerance)
				return mid;

			if (fmid < p)
				low = mid;
			else
				high = mid;
		}

		return (low + high) / 2;
	}

	public override double ProbabilityDensity(double x)
	{
		switch (x)
		{
			case < 0: return 0;
			case 0 when Shape == 1: return Rate;
			case 0: return 0;
		}

		var numerator = Math.Pow(Rate, Shape) * Math.Pow(x, Shape - 1) * Math.Exp(-Rate * x);
		var denominator = FactorialFunction.Calculate(Shape - 1);
		
		return numerator / denominator;
	}

	public override double CumulativeDistribution(double x)
	{
		if (x < 0)
			return 0;

		double sum = 0;
		
		for (var i = 0; i < Shape; i++)
		{
			sum += Math.Pow(Rate * x, i) / FactorialFunction.Calculate(i);
		}
		
		return 1 - Math.Exp(-Rate * x) * sum;
	}
	
	public override string ToString() => $"Erlang Distribution [Shape = {Shape}, Rate = {Rate}]";

	public override bool Equals(object? obj) => obj is ErlangDistribution other && Shape == other.Shape && DoubleUtils.Approximately(Rate, other.Rate);

	public override int GetHashCode() => HashCode.Combine(Shape, Rate);
}