using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Discrete.Infinite;

public partial class GeometricDistribution : Distribution
{
    private readonly double _probability;
    
    public GeometricDistribution(double probability = 0.5)
    {
        if (probability is <= 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1.");

        _probability = probability;
    }
    
    public override double Expected => 1.0 / _probability;
    
    public override double Mean => 1.0 / _probability;
    
    public override double Median => Math.Ceiling(-Math.Log(2) / Math.Log(1 - _probability));
    
    public override double Mode => 1;
    
    public override double Variance => (1 - _probability) / (_probability * _probability);
    
    public override double Skewness => (2 - _probability) / Math.Sqrt(1 - _probability);
    
    public override double Kurtosis => 6 + (_probability * _probability) / (1 - _probability);
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 1;
    
    public override double Maximum => double.PositiveInfinity;

    public double Probability => _probability;
    
    public double P => _probability;
    
    public double Q => 1 - _probability;
    
    public override double Distribute() => Math.Floor(Math.Log(1 - RandomUtils.NextDouble()) / Math.Log(1 - _probability)) + 1;

    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        return p switch
        {
            0 => 1,
            1 => double.PositiveInfinity,
            _ => Math.Ceiling(Math.Log(1 - p) / Math.Log(1 - _probability))
        };
    }
    
    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < 1 || x != k)
            return 0;

        return ProbabilityMass(k);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 1)
            return 0;

        var floorX = (int)Math.Floor(x);
        return 1 - Math.Pow(1 - _probability, floorX);
    }

    // Функция массы вероятности для геометрического распределения
    public double ProbabilityMass(int k)
    {
        if (k < 1)
            return 0;

        return _probability * Math.Pow(1 - _probability, k - 1);
    }

    public double Moment(int r)
    {
        if (r < 0)
            throw new ArgumentOutOfRangeException(nameof(r), "Moment order must be non-negative");

        if (r == 0)
            return 1;

        var p = _probability;
        var q = 1 - p;
        
        switch (r)
        {
            case 1: return 1 / p;
            case 2: return (2 - p) / (p * p);
            case 3: return (6 - 6 * p + p * p) / (p * p * p);
        }
        
        var moment = 0.0;
        for (var k = 1; k <= 1000; k++)
        {
            var term = Math.Pow(k, r) * p * Math.Pow(q, k - 1);
            moment += term;
            if (term < 1e-15 * moment)
                break;
        }
        return moment;
    }
    
    public override string ToString() => $"Geometric Distribution [Probability = {_probability}]";

    public override bool Equals(object obj) => obj is GeometricDistribution other && _probability == other._probability;

    public override int GetHashCode() => _probability.GetHashCode();
}