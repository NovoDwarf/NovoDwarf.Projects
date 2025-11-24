using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Discrete.Finite;

public partial class BinomialDistribution : Distribution
{
    private readonly int _trials;
    private readonly double _probability;
    
    public BinomialDistribution(int trials = 10, double probability = 0.5)
    {
        if (trials <= 0)
            throw new ArgumentOutOfRangeException(nameof(trials), "Number of trials must be positive.");

        if (probability is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1.");

        _trials = trials;
        _probability = probability;
    }
    
    public override double Expected => _trials * _probability;
    
    public override double Mean => _trials * _probability;
    
    public override double Median => GetMedian();
    
    public override double Mode => GetMode();
    
    public override double Variance => _trials * _probability * (1 - _probability);
    
    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => _trials;
    
    public int Trials => _trials;
    
    public double Probability => _probability;
    
    public double P => _probability;
    
    public double Q => 1 - _probability;
    
    public int N => _trials;

    public override double Distribute()
    {
        var successes = 0;

        for (var i = 0; i < _trials; i++)
        {
            if (RandomUtils.NextDouble() < _probability)
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
            case 1: return _trials;
        }

        var cumulative = 0.0;
        for (var k = 0; k <= _trials; k++)
        {
            cumulative += ProbabilityMass(k);
            if (p <= cumulative)
                return k;
        }
        
        return _trials;
    }
    
    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < 0 || k > _trials || x != k)
            return 0;

        return ProbabilityMass(k);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;
            
        if (x >= _trials)
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
        if (k < 0 || k > _trials)
            return 0;

        // P(X = k) = C(n, k) * p^k * (1-p)^(n-k)
        var binomialCoeff = BinomialCoefficient(_trials, k);
        var probability = binomialCoeff * Math.Pow(_probability, k) * Math.Pow(1 - _probability, _trials - k);
        
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
    
    public override string ToString() => $"Binomial Distribution [Trials = {_trials}, Probability = {_probability}]";

    public override bool Equals(object obj) => obj is BinomialDistribution other && _trials == other._trials && _probability == other._probability;

    public override int GetHashCode() => HashCode.Combine(_trials, _probability);
    
    private double GetMedian()
    {
        var np = _trials * _probability;
            
        return np % 1 == 0.5 ? np : Math.Floor(np);
    }
    
    private double GetMode()
    {
        var mode = Math.Floor((_trials + 1) * _probability);

        if (mode < 0)
            return 0;
        if (mode > _trials)
            return _trials;
        return mode;
    }
    
    private double GetSkewness()
    {
        if (_probability == 0 || _probability == 1)
            return double.NaN;
                
        return (1 - 2 * _probability) / Math.Sqrt(_trials * _probability * (1 - _probability));
    }
    
    private double GetKurtosis()
    {
        if (_probability == 0 || _probability == 1)
            return double.NaN;
                
        return (1 - 6 * _probability * (1 - _probability)) / (_trials * _probability * (1 - _probability));
    }
}