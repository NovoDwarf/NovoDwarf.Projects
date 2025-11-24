using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Discrete.Finite;

public partial class BernoulliDistribution : Distribution
{
    private readonly double _probability;
    
    public BernoulliDistribution(double probability = 0.5)
    {
        if (probability is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(probability), "Probability must be between 0 and 1.");

        _probability = probability;
    }

    public override double Expected => _probability;
    
    public override double Mean => _probability;
    
    public override double Median
    {
        get
        {
            return _probability switch
            {
                < 0.5 => 0,
                > 0.5 => 1,
                _ => 0.5
            };
        }
    }
    
    public override double Mode
    {
        get
        {
            return _probability switch
            {
                > 0.5 => 1,
                < 0.5 => 0,
                _ => double.NaN
            };
        }
    }

    public override double Variance => _probability * (1 - _probability);
    
    public override double Skewness =>
        _probability is 0 or 1
            ? double.NaN
            : (1 - 2 * _probability) / Math.Sqrt(_probability * (1 - _probability));

    public override double Kurtosis =>
        _probability is 0 or 1
            ? double.NaN
            : (1 - 6 * _probability * (1 - _probability)) / (_probability * (1 - _probability));

    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => 1;

    public override double Distribute() => RandomUtils.NextDouble() < _probability ? 1.0 : 0.0;

    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        return p <= 1 - _probability ? 0 : 1;
    }

    
    public override double ProbabilityDensity(double x)
    {
        return x switch
        {
            0 => 1 - _probability,
            1 => _probability,
            _ => 0
        };
    }

    public override double CumulativeDistribution(double x)
    {
        return x switch
        {
            < 0 => 0,
            < 1 => 1 - _probability,
            _ => 1
        };
    }

    // Дополнительные полезные методы
    public double ProbabilityMass(int k)
    {
        return k switch
        {
            0 => 1 - _probability,
            1 => _probability,
            _ => 0
        };
    }
    
    public double Probability => _probability;
    public double P => _probability;
    public double Q => 1 - _probability;
    
    public override string ToString() => $"Bernoulli Distribution [Probability = {_probability}]";

    public override bool Equals(object obj) => obj is BernoulliDistribution other && _probability == other._probability;

    public override int GetHashCode() => _probability.GetHashCode();
}