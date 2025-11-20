using Mathematics.Core.Distributions.Base;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class ExpoHypoDistribution : Distribution
{
	private readonly ExpoDistribution[] _stages;
	private readonly double[] _rates;
	
	public ExpoHypoDistribution(double[] rates)
	{
		if (rates == null || rates.Length == 0)
			throw new ArgumentException("At least one rate is required", nameof(rates));

		if (rates.Any(r => r <= 0))
			throw new ArgumentException("All rates must be positive", nameof(rates));

		_rates = rates;
		_stages = rates.Select(rate => new ExpoDistribution(rate)).ToArray();
	}

	public override double Expected { get; }
	public override double Mean { get; }
	public override double Median { get; }
	public override double Mode { get; }
	public override double Variance { get; }
	public override double Skewness { get; }
	public override double Kurtosis { get; }
	public override double StandardDeviation { get; }
	public override double Minimum { get; }
	public override double Maximum { get; }

	public override double Distribute()
	{
		return _stages.Sum(stage => stage.Distribute());
	}

	public override double ProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double CumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}
}