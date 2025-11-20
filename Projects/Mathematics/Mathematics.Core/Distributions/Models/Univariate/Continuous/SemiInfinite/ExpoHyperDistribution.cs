using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class ExpoHyperDistribution : Distribution
{
	private readonly ExpoDistribution[] _components;
	private readonly double[] _expoProbabilities;

	private readonly double[] _probabilities;
	private readonly double[] _rates;
	
	public ExpoHyperDistribution(double[] probabilities, double[] rates)
	{
		if (probabilities == null || rates == null)
			throw new ArgumentNullException("Probabilities and rates cannot be null");

		if (probabilities.Length != rates.Length)
			throw new ArgumentException("Probabilities and rates must have same length");

		if (probabilities.Length == 0)
			throw new ArgumentException("At least one component required");

		if (probabilities.Any(p => p < 0))
			throw new ArgumentException("Probabilities must be non-negative");

		if (rates.Any(r => r <= 0))
			throw new ArgumentException("Rates must be positive");

		_probabilities = probabilities;
		_rates = rates;

		var sum = _probabilities.Sum();

		if (sum <= 0)
			throw new ArgumentException("Sum of probabilities must be positive");

		_expoProbabilities = _probabilities.Select(p => p / sum).ToArray();
		_components = _rates.Select(r => new ExpoDistribution(r)).ToArray();
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
		var r = RandomUtils.NextDouble();
		var cumulative = 0.0;
		var index = _expoProbabilities.Length - 1;

		for (var i = 0; i < _expoProbabilities.Length; i++)
		{
			cumulative += _expoProbabilities[i];

			if (r < cumulative || i == _expoProbabilities.Length - 1)
			{
				index = i;
				break;
			}
		}

		return _components[index].Distribute();
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