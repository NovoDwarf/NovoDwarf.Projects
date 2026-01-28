using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Discrete.Finite;

[Categories("Distributions", "Univariate", "Discrete", "Finite")]
public partial class GeometricHyperDistribution : Distribution
{
    public override double Expected => Draws * SuccessStates / (double)PopulationSize;
    
    public override double Mean => Draws * SuccessStates / (double)PopulationSize;
    
    public override double Median => Math.Floor((Draws + 1) * (SuccessStates + 1) / (double)(PopulationSize + 2));
    
    public override double Mode => Math.Floor((Draws + 1) * (SuccessStates + 1) / (double)(PopulationSize + 2));

    public override double Variance => GetVariance();
    
    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => Math.Max(0, Draws - (PopulationSize - SuccessStates));
    
    public override double Maximum => Math.Min(Draws, SuccessStates);

    [EntityParameter(typeof(int), nameof(PopulationSize))]
    public int PopulationSize { get; private set; } = 20;

    [EntityParameter(typeof(int), nameof(SuccessStates))]
    public int SuccessStates { get; private set; } = 5;

    [EntityParameter(typeof(int), nameof(Draws))]
    public int Draws { get; private set; } = 10;

    protected override void Validate()
    {
        if (PopulationSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(PopulationSize), "Population size must be positive.");

        if (SuccessStates < 0 || SuccessStates > PopulationSize)
            throw new ArgumentOutOfRangeException(nameof(SuccessStates), "Success states must be between 0 and population size.");

        if (Draws < 0 || Draws > PopulationSize)
            throw new ArgumentOutOfRangeException(nameof(Draws), "Number of draws must be between 0 and population size.");
    }

    public override double Distribute()
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
    
    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return Minimum;
            case 1: return Maximum;
        }

        var cumulative = 0.0;
        
        for (var k = (int)Minimum; k <= Maximum; k++)
        {
            cumulative += ProbabilityMass(k);
            
            if (p <= cumulative)
                return k;
        }
        
        return Maximum;
    }

    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < Minimum || k > Maximum || x != k)
            return 0;

        return ProbabilityMass(k);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < Minimum)
            return 0;
            
        if (x >= Maximum)
            return 1;

        var floorX = (int)Math.Floor(x);
        var cdf = 0.0;
        
        for (var k = (int)Minimum; k <= floorX; k++)
        {
            cdf += ProbabilityMass(k);
        }
        
        return cdf;
    }
    
    public double ProbabilityMass(int k)
    {
        if (k < Minimum || k > Maximum)
            return 0;

        var numerator = BinomialCoefficient(SuccessStates, k) * BinomialCoefficient(PopulationSize - SuccessStates, Draws - k);
        var denominator = BinomialCoefficient(PopulationSize, Draws);
        
        return numerator / denominator;
    }

    private static double BinomialCoefficient(int n, int k)
    {
        if (k < 0 || k > n)
            return 0;
            
        if (k == 0 || k == n)
            return 1;

        k = Math.Min(k, n - k);
        double result = 1;
        
        for (var i = 1; i <= k; i++)
        {
            result *= (n - k + i) / (double)i;
        }
        
        return result;
    }
    
    public override string ToString() => $"Geometric Hyper Distribution [Population = {PopulationSize}, Successes = {SuccessStates}, Draws = {Draws}]";

    public override bool Equals(object? obj) => obj is GeometricHyperDistribution other && PopulationSize == other.PopulationSize && SuccessStates == other.SuccessStates && Draws == other.Draws;

    public override int GetHashCode() => HashCode.Combine(PopulationSize, SuccessStates, Draws);
    
    private double GetVariance()
    {
        return Draws * (SuccessStates / (double)PopulationSize) * ((PopulationSize - SuccessStates) / (double)PopulationSize) * ((PopulationSize - Draws) / (double)(PopulationSize - 1));
    }
    
    private double GetSkewness()
    {
        var p = SuccessStates / (double)PopulationSize;
        var q = 1 - p;
            
        var numerator = (PopulationSize - 2 * SuccessStates) * Math.Sqrt(PopulationSize - 1) * (PopulationSize - 2 * Draws);
        var denominator = Math.Sqrt(Draws * SuccessStates * (PopulationSize - SuccessStates) * (PopulationSize - Draws)) * (PopulationSize - 2);
            
        return numerator / denominator;
    }
    
    private double GetKurtosis()
    {
        var p = SuccessStates / (double)PopulationSize;
        var q = 1 - p;
            
        var term1 = (PopulationSize - 1) * (PopulationSize * PopulationSize) * (PopulationSize * (PopulationSize + 1) - 6 * SuccessStates * (PopulationSize - SuccessStates) - 6 * Draws * (PopulationSize - Draws));
        var term2 = 6 * Draws * SuccessStates * (PopulationSize - SuccessStates) * (PopulationSize - Draws) * (5 * PopulationSize - 6);
        var denominator = Draws * SuccessStates * (PopulationSize - SuccessStates) * (PopulationSize - Draws) * (PopulationSize - 2) * (PopulationSize - 3);
            
        return (term1 - term2) / denominator;
    }
}