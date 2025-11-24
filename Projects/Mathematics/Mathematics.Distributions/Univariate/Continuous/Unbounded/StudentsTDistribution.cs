using Mathematics.Core.Base;
using Mathematics.Functions;
using Mathematics.Functions.Transforms;

namespace Mathematics.Distributions.Univariate.Continuous.Unbounded;

public partial class StudentsTDistribution : Distribution
{
	private readonly int _degreesOfFreedom;
	
	public StudentsTDistribution(int degreesOfFreedom = 10)
	{
		if (degreesOfFreedom <= 0)
			throw new ArgumentOutOfRangeException(nameof(degreesOfFreedom), "Degrees of freedom must be positive");

		_degreesOfFreedom = degreesOfFreedom;
	}

	public override double Expected => _degreesOfFreedom > 1 ? 0 : double.NaN;
	
	public override double Mean => _degreesOfFreedom > 1 ? 0 : double.NaN;
	
	public override double Median => 0;
	
	public override double Mode => 0;
	
	public override double Variance => GetVariance();

	public override double Skewness => _degreesOfFreedom > 3 ? 0 : double.NaN;

	public override double Kurtosis => GetKurtosis();

	public override double StandardDeviation => Math.Sqrt(Variance);
	
	public override double Minimum => double.NegativeInfinity;
	
	public override double Maximum => double.PositiveInfinity;

	public override double Distribute()
	{
		var (z, _) = BoxMullerPolarTransform.Transform();

		var v = 0.0;

		for (var i = 0; i < _degreesOfFreedom; i++)
		{
			var (zi, _) = BoxMullerPolarTransform.Transform();
			v += zi * zi;
		}

		return z / Math.Sqrt(v / _degreesOfFreedom);
	}

	public override double Quantile(double p)
	{
		if (p <= 0 || p >= 1)
			throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

		if (p < 0.5)
			return -Quantile(1 - p);

		var nu = (double)_degreesOfFreedom;
		var x = InverseRegularizedIncompleteBeta(2 * (1 - p), nu / 2, 0.5);
		
		return Math.Sqrt(nu * (1 - x) / x);
	}
	
	public override double ProbabilityDensity(double x)
	{
		var nu = (double)_degreesOfFreedom;
		var numerator = GammaFunction.Calculate((nu + 1) / 2);
		var denominator = Math.Sqrt(nu * Math.PI) * GammaFunction.Calculate(nu / 2);
		var power = Math.Pow(1 + (x * x) / nu, -(nu + 1) / 2);
		
		return numerator / denominator * power;
	}

	public override double CumulativeDistribution(double x)
	{
		var nu = (double)_degreesOfFreedom;
		var t = (x + Math.Sqrt(x * x + nu)) / (2 * Math.Sqrt(x * x + nu));
		
		return RegularizedIncompleteBetaFunction.Calculate(t, nu / 2, nu / 2);
	}

	private static double InverseRegularizedIncompleteBeta(double y, double a, double b)
	{
		double low = 0, high = 1;
		
		const double tolerance = 1e-10;
		const int maxIterations = 100;

		for (var i = 0; i < maxIterations; i++)
		{
			var mid = (low + high) / 2;
			var fmid = RegularizedIncompleteBetaFunction.Calculate(mid, a, b);

			if (Math.Abs(fmid - y) < tolerance)
				return mid;

			if (fmid < y)
				low = mid;
			else
				high = mid;
		}

		return (low + high) / 2;
	}

	public override string ToString() => $"Students T Distribution [DegreesOfFreedom = {_degreesOfFreedom}]";

	public override bool Equals(object? obj) => obj is StudentsTDistribution other && _degreesOfFreedom == other._degreesOfFreedom;

	public override int GetHashCode() => _degreesOfFreedom.GetHashCode();

	private double GetVariance()
	{
		return _degreesOfFreedom switch
		{
			> 2 => _degreesOfFreedom / (double)(_degreesOfFreedom - 2),
			> 1 => double.PositiveInfinity,
			_ => double.NaN
		};
	}

	private double GetKurtosis()
	{
		return _degreesOfFreedom switch
		{
			> 4 => 6.0 / (_degreesOfFreedom - 4),
			> 2 => double.PositiveInfinity,
			_ => double.NaN
		};
	}
}