using Mathematics.Core.Base;
using Mathematics.Functions;

namespace Mathematics.Distributions.Univariate.Continuous.Semibounded;

public partial class ErlangDistribution : Distribution
{
	private readonly int _shape;
	private readonly double _rate;
	
	public ErlangDistribution(int shape = 2, double rate = 1)
	{
		if (shape < 1)
			throw new ArgumentException("Shape must be greater than 0.", nameof(shape));

		if (rate <= 0)
			throw new ArgumentException("Rate must be greater than 0.", nameof(rate));

		_shape = shape;
		_rate = rate;
	}

	public override double Expected => _shape / _rate;
	
	public override double Mean => _shape / _rate;
	
	public override double Median
	{
		get
		{
			// Медиана распределения Эрланга не имеет простой аналитической формы
			// Используем приближение через обратную гамма-функцию
			if (_shape == 1)
				return Math.Log(2) / _rate; // Для экспоненциального распределения
			
			// Приближение для медианы
			return (_shape - 1.0 / 3.0) / _rate;
		}
	}
	
	public override double Mode => _shape >= 1 ? (_shape - 1) / _rate : 0;

	public override double Variance => _shape / (_rate * _rate);
	
	public override double Skewness => 2.0 / Math.Sqrt(_shape);
	
	public override double Kurtosis => 6.0 / _shape;
	
	public override double StandardDeviation => Math.Sqrt(Variance);
	
	public override double Minimum => 0;
	
	public override double Maximum => double.PositiveInfinity;

	public override double Distribute()
	{
		var sum = 0.0;
		var exp = new ExpoDistribution(_rate);

		for (var i = 0; i < _shape; i++)
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
			case 0 when _shape == 1: return _rate;
			case 0: return 0;
		}

		var numerator = Math.Pow(_rate, _shape) * Math.Pow(x, _shape - 1) * Math.Exp(-_rate * x);
		var denominator = FactorialFunction.Calculate(_shape - 1);
		
		return numerator / denominator;
	}

	public override double CumulativeDistribution(double x)
	{
		if (x < 0)
			return 0;

		double sum = 0;
		
		for (var i = 0; i < _shape; i++)
		{
			sum += Math.Pow(_rate * x, i) / FactorialFunction.Calculate(i);
		}
		
		return 1 - Math.Exp(-_rate * x) * sum;
	}
	
	private static double LowerIncompleteGamma(int s, double x)
	{
		// Нижняя неполная гамма-функция для целого s
		return GammaFunction.Calculate(s) * (1 - Math.Exp(-x) * SeriesSum(s, x));
	}

	private static double SeriesSum(int s, double x)
	{
		double sum = 0;
		for (var k = 0; k < s; k++)
		{
			sum += Math.Pow(x, k) / FactorialFunction.Calculate(k);
		}
		return sum;
	}
	
	public override string ToString() => $"Erlang Distribution [Shape = {_shape}, Rate = {_rate}]";

	public override bool Equals(object? obj) => obj is ErlangDistribution other && _shape == other._shape && _rate == other._rate;

	public override int GetHashCode() => HashCode.Combine(_shape, _rate);
}