using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Numerical.Simple;
using Mathematics.Numerical.Transforms;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Unbounded;

[Categories("Distributions", "Univariate", "Continious", "Unbounded")]
public partial class StudentsTDistribution : Distribution
{
	public override double Expected => DegreesOfFreedom > 1 ? 0 : double.NaN;
	
	public override double Mean => DegreesOfFreedom > 1 ? 0 : double.NaN;
	
	public override double Median => 0;
	
	public override double Mode => 0;
	
	public override double Variance => GetVariance();

	public override double Skewness => DegreesOfFreedom > 3 ? 0 : double.NaN;

	public override double Kurtosis => GetKurtosis();

	public override double StandardDeviation => Math.Sqrt(Variance);
	
	public override double Minimum => double.NegativeInfinity;
	
	public override double Maximum => double.PositiveInfinity;

	[EntityParameter(typeof(int), nameof(DegreesOfFreedom))]
	public int DegreesOfFreedom { get; private set; } = 10;

	protected override void Validate()
	{
		ArgumentOutOfRangeException.ThrowIfNegative(DegreesOfFreedom);
	}

	public override double Distribute()
	{
		var (z, _) = BoxMullerPolarTransform.Transform();

		var v = 0.0;

		for (var i = 0; i < DegreesOfFreedom; i++)
		{
			var (zi, _) = BoxMullerPolarTransform.Transform();
			v += zi * zi;
		}

		return z / Math.Sqrt(v / DegreesOfFreedom);
	}

	public override double Quantile(double p)
	{
		if (p <= 0 || p >= 1)
			throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

		if (p < 0.5)
			return -Quantile(1 - p);

		var nu = (double)DegreesOfFreedom;
		var x = InverseRegularizedIncompleteBeta(2 * (1 - p), nu / 2, 0.5);
		
		return Math.Sqrt(nu * (1 - x) / x);
	}
	
	public override double ProbabilityDensity(double x)
	{
		var nu = (double)DegreesOfFreedom;
		var numerator = GammaFunction.Calculate((nu + 1) / 2);
		var denominator = Math.Sqrt(nu * Math.PI) * GammaFunction.Calculate(nu / 2);
		var power = Math.Pow(1 + (x * x) / nu, -(nu + 1) / 2);
		
		return numerator / denominator * power;
	}

	public override double CumulativeDistribution(double x)
	{
		var nu = (double)DegreesOfFreedom;
		var t = (x + Math.Sqrt(x * x + nu)) / (2 * Math.Sqrt(x * x + nu));
		
		return BetaRegularizedIncompleteFunction.Calculate(t, nu / 2, nu / 2);
	}

	private static double InverseRegularizedIncompleteBeta(double y, double a, double b)
	{
		double low = 0, high = 1;
		
		const double tolerance = 1e-10;
		const int maxIterations = 100;

		for (var i = 0; i < maxIterations; i++)
		{
			var mid = (low + high) / 2;
			var fmid = BetaRegularizedIncompleteFunction.Calculate(mid, a, b);

			if (Math.Abs(fmid - y) < tolerance)
				return mid;

			if (fmid < y)
				low = mid;
			else
				high = mid;
		}

		return (low + high) / 2;
	}

	public override string ToString() => $"Students T Distribution [DegreesOfFreedom = {DegreesOfFreedom}]";

	public override bool Equals(object? obj) => obj is StudentsTDistribution other && DegreesOfFreedom == other.DegreesOfFreedom;

	public override int GetHashCode() => DegreesOfFreedom.GetHashCode();

	private double GetVariance()
	{
		return DegreesOfFreedom switch
		{
			> 2 => DegreesOfFreedom / (double)(DegreesOfFreedom - 2),
			> 1 => double.PositiveInfinity,
			_ => double.NaN
		};
	}

	private double GetKurtosis()
	{
		return DegreesOfFreedom switch
		{
			> 4 => 6.0 / (DegreesOfFreedom - 4),
			> 2 => double.PositiveInfinity,
			_ => double.NaN
		};
	}
}