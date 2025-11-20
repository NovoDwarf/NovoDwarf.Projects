using Mathematics.Distributions.Base;

namespace Mathematics.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class ExpoHypoDistribution : Distribution
{
	private readonly ExpoDistribution[] _stages;

	public ExpoHypoDistribution(double[] rates)
	{
		if (rates == null || rates.Length == 0)
			throw new ArgumentException("At least one rate is required", nameof(rates));

		if (rates.Any(r => r <= 0))
			throw new ArgumentException("All rates must be positive", nameof(rates));

		Rates = rates;

		_stages = rates.Select(rate => new ExpoDistribution(rate)).ToArray();
	}

	public double[] Rates { get; }

	public override double Calculate()
	{
		return _stages.Sum(stage => stage.Calculate());
	}

	public override double GetProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetCumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetExpectedValue()
	{
		return _stages.Sum(s => s.GetExpectedValue());
	}

	public override double GetMean()
	{
		throw new NotImplementedException();
	}

	public override double GetMedian()
	{
		throw new NotImplementedException();
	}

	public override double GetMode()
	{
		throw new NotImplementedException();
	}

	public override double GetVariance()
	{
		return _stages.Sum(s => s.GetVariance());
	}

	public override double GetSkewness()
	{
		throw new NotImplementedException();
	}

	public override double GetKurtosis()
	{
		throw new NotImplementedException();
	}

	public override double GetStandardDeviation()
	{
		throw new NotImplementedException();
	}

	public override double GetMinValue()
	{
		return 0;
	}

	public override double GetMaxValue()
	{
		return double.PositiveInfinity;
	}

	public override string ToString()
	{
		return $"HypoExponential(rates=[{string.Join(", ", Rates.Select(r => r.ToString("F3")))}])";
	}
}