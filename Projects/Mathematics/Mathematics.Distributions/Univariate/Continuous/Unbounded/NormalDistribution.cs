using Mathematics.Core.Base;
using Mathematics.Core.Utilities;
using Mathematics.Functions;
using Mathematics.Randoms;

namespace Mathematics.Distributions.Univariate.Continuous.Unbounded;

public partial class NormalDistribution : Distribution
{
	private readonly double _mean;
	private readonly double _stdDev;
	
	public NormalDistribution(double mean = 0, double stdDev = 1)
	{
		if (stdDev <= 0)
			throw new ArgumentOutOfRangeException(nameof(stdDev), "Standard deviation must be positive");

		_mean = mean;
		_stdDev = stdDev;
	}

	public override double Expected => _mean;
	
	public override double Mean => _mean;
	
	public override double Median => _mean;
	
	public override double Mode => _mean;
	
	public override double Variance => _stdDev * _stdDev;
	
	public override double Skewness => 0;
	
	public override double Kurtosis => 0;
	
	public override double StandardDeviation => _stdDev;
	
	public override double Minimum => double.NegativeInfinity;
	
	public override double Maximum => double.PositiveInfinity;
	
	public override double Distribute() => _mean + _stdDev * RandomUtils.NextNormal();
	
	public override double ProbabilityDensity(double x)
	{
		var exponent = -0.5 * Math.Pow((x - _mean) / _stdDev, 2);
		
		return Math.Exp(exponent) / (_stdDev * Math.Sqrt(2 * Math.PI));
	}

	public override double CumulativeDistribution(double x)
	{
		var z = (x - _mean) / (_stdDev * Math.Sqrt(2));
		
		return 0.5 * (1 + ErrorFunction.Calculate(z));
	}

	public override double Quantile(double p)
	{
		if (p <= 0 || p >= 1)
			throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

		// Аппроксимация обратной функции нормального распределения
		// Используем приближение Мора (Morr)
		return p < 0.5 ? _mean - _stdDev * InverseNormalCDF(1 - p) : _mean + _stdDev * InverseNormalCDF(p);
	}

	private static double InverseNormalCDF(double p)
	{
		// Аппроксимация обратной функции нормального распределения
		// Peter J. Acklam's algorithm
		if (p <= 0 || p >= 1)
			throw new ArgumentOutOfRangeException(nameof(p));

		double[] a = { -3.969683028665376e+01, 2.209460984245205e+02,
					  -2.759285104469687e+02, 1.383577518672690e+02,
					  -3.066479806614716e+01, 2.506628277459239e+00 };

		double[] b = { -5.447609879822406e+01, 1.615858368580409e+02,
					  -1.556989798598866e+02, 6.680131188771972e+01,
					  -1.328068155288572e+01 };

		double[] c = { -7.784894002430293e-03, -3.223964580411365e-01,
					  -2.400758277161838e+00, -2.549732539343734e+00,
					  4.374664141464968e+00, 2.938163982698783e+00 };

		double[] d = { 7.784695709041462e-03, 3.224671290700398e-01,
					  2.445134137142996e+00, 3.754408661907416e+00 };

		double q, r;

		if (p < 0.02425)
		{
			// Rational approximation for lower region
			q = Math.Sqrt(-2 * Math.Log(p));
			return (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
				   ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
		}
		else if (p > 0.97575)
		{
			// Rational approximation for upper region
			q = Math.Sqrt(-2 * Math.Log(1 - p));
			return -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
					((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
		}
		else
		{
			// Rational approximation for central region
			q = p - 0.5;
			r = q * q;
			return (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q /
				   (((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1);
		}
	}

	public override string ToString() => $"Normal Distribution [Mean = {_mean}, StdDev = {_stdDev}]";

	public override bool Equals(object? obj) => obj is NormalDistribution other && _mean == other._mean && _stdDev == other._stdDev;

	public override int GetHashCode() => HashCode.Combine(_mean, _stdDev);
}