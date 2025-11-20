using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Discrete.Finite;

public partial class BinomialDistribution : Distribution
{
	public BinomialDistribution(int trials, double probability)
	{
		if (trials <= 0)
			throw new ArgumentOutOfRangeException(nameof(trials), "Number of trials must be positive.");
        
		if (probability is < 0 or > 1)
			throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1.");

		Trials = trials;
		Probability = probability;
	}

	public int Trials { get; }
	public double Probability { get; }

	public override double Calculate()
	{
		var successes = 0;
		
		for (var i = 0; i < Trials; i++)
		{
			if (RandomUtils.NextDouble() < Probability)
				successes++;
		}
		
		return successes;
	}

	public override double GetProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetCumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetExpectedValue() => Trials * Probability;
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

	public override double GetVariance() => Trials * Probability * (1 - Probability);
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

	public override double GetMinValue() => 0.0;

	public override double GetMaxValue() => Trials;
}