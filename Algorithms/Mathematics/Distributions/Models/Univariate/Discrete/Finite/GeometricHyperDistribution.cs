using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Discrete.Finite;

public partial class GeometricHyperDistribution : Distribution
{
    public GeometricHyperDistribution(int populationSize, int successStates, int draws)
    {
        if (populationSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(populationSize), "Population size must be positive.");
        
        if (successStates < 0 || successStates > populationSize)
            throw new ArgumentOutOfRangeException(nameof(successStates), "Success states must be between 0 and population size.");
        
        if (draws < 0 || draws > populationSize)
            throw new ArgumentOutOfRangeException(nameof(draws), "Number of draws must be between 0 and population size.");

        PopulationSize = populationSize;
        SuccessStates = successStates;
        Draws = draws;
    }

    public int PopulationSize { get; }
    public int SuccessStates { get; }
    public int Draws { get; }

    public override double Calculate()
    {
        var successes = 0;
        var remainingSuccesses = SuccessStates;
        var remainingPopulation = PopulationSize;

        for (var i = 0; i < Draws; i++)
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

    public override double GetProbabilityDensity(double x)
    {
        throw new NotImplementedException();
    }

    public override double GetCumulativeDistribution(double x)
    {
        throw new NotImplementedException();
    }

    public override double GetExpectedValue() => Draws * ((double)SuccessStates / PopulationSize);
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
        var p = (double)SuccessStates / PopulationSize;
        return Draws * p * (1 - p) * ((double)(PopulationSize - Draws) / (PopulationSize - 1));
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

    public override double GetMinValue() => Math.Max(0, Draws - (PopulationSize - SuccessStates));

    public override double GetMaxValue() => Math.Min(Draws, SuccessStates);
}