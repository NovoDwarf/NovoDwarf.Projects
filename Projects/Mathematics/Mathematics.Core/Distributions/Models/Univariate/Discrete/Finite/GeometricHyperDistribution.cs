using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;

namespace Mathematics.Core.Distributions.Models.Univariate.Discrete.Finite;

public partial class GeometricHyperDistribution : Distribution
{
	private readonly int _populationSize;
	private readonly int _successStates;
	private readonly int _draws;
	
	public GeometricHyperDistribution(int populationSize, int successStates, int draws)
	{
		if (populationSize <= 0)
			throw new ArgumentOutOfRangeException(nameof(populationSize), "Population size must be positive.");

		if (successStates < 0 || successStates > populationSize)
			throw new ArgumentOutOfRangeException(nameof(successStates),
				"Success states must be between 0 and population size.");

		if (draws < 0 || draws > populationSize)
			throw new ArgumentOutOfRangeException(nameof(draws),
				"Number of draws must be between 0 and population size.");

		_populationSize = populationSize;
		_successStates = successStates;
		_draws = draws;
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
		var successes = 0;
		var remainingSuccesses = _successStates;
		var remainingPopulation = _populationSize;

		for (var i = 0; i < _draws; i++)
		{
			var probability = (double)remainingSuccesses / remainingPopulation;

			if (RandomUtils.NextDouble() < probability)
			{
				successes++;
				remainingSuccesses--;
			}

			remainingPopulation--;
		}

		return successes;
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