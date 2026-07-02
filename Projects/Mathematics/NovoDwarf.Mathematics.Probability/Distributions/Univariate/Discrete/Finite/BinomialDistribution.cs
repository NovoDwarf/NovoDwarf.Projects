using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Discrete.Finite;

[Categories("Distributions", "Univariate", "Discrete", "Finite")]
public partial class BinomialDistribution : Distribution
{
    public override double Expected => Trials * Probability;
    
    public override double Mean => Trials * Probability;
    
    public override double Median => GetMedian();
    
    public override double Mode => GetMode();
    
    public override double Variance => Trials * Probability * (1 - Probability);
    
    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => Trials;

    [EntityParameter(typeof(int), nameof(Trials))]
    public int Trials { get; private set; } = 10;

    [EntityParameter(typeof(double), nameof(Probability))]
    public double Probability { get; private set; } = 0.5;
    
    public double Q => 1 - Probability;

    protected override void Validate()
    {
        if (Trials <= 0)
            throw new ArgumentOutOfRangeException(nameof(Trials), "Number of trials must be positive.");

        if (Probability is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(Probability), "Probability must be between 0 and 1.");
    }
    
    public override double Distribute()
    {
        var successes = 0;

        for (var i = 0; i < Trials; i++)
        {
            if (RandomUtils.NextDouble() < Probability)
            {
                successes++;
            }
        }

        return successes;
    }

    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return 0;
            case 1: return Trials;
        }

        var cumulative = 0.0;
        for (var k = 0; k <= Trials; k++)
        {
            cumulative += ProbabilityMass(k);
            if (p <= cumulative)
                return k;
        }
        
        return Trials;
    }
    
    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < 0 || k > Trials || x != k)
            return 0;

        return ProbabilityMass(k);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;
            
        if (x >= Trials)
            return 1;

        var floorX = (int)Math.Floor(x);
        var cdf = 0.0;
        
        for (var k = 0; k <= floorX; k++)
        {
            cdf += ProbabilityMass(k);
        }
        
        return cdf;
    }
    
    public double ProbabilityMass(int k)
    {
        if (k < 0 || k > Trials)
            return 0;

        var binomialCoeff = BinomialCoefficient(Trials, k);
        var probability = binomialCoeff * Math.Pow(Probability, k) * Math.Pow(1 - Probability, Trials - k);
        
        return probability;
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
    
    public override string ToString() => $"Binomial Distribution [Trials = {Trials}, Probability = {Probability}]";

    public override bool Equals(object obj) => obj is BinomialDistribution other && Trials == other.Trials && Probability == other.Probability;

    public override int GetHashCode() => HashCode.Combine(Trials, Probability);
    
    private double GetMedian()
    {
        var np = Trials * Probability;
            
        return np % 1 == 0.5 ? np : Math.Floor(np);
    }
    
    private double GetMode()
    {
        var mode = Math.Floor((Trials + 1) * Probability);

        if (mode < 0)
            return 0;
        if (mode > Trials)
            return Trials;
        return mode;
    }
    
    private double GetSkewness()
    {
        if (Probability == 0 || Probability == 1)
            return double.NaN;
                
        return (1 - 2 * Probability) / Math.Sqrt(Trials * Probability * (1 - Probability));
    }
    
    private double GetKurtosis()
    {
        if (Probability == 0 || Probability == 1)
            return double.NaN;
                
        return (1 - 6 * Probability * (1 - Probability)) / (Trials * Probability * (1 - Probability));
    }
}