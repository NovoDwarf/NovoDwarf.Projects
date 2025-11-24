using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Discrete.Finite;

public partial class UniformDiscreteDistribution : Distribution
{
    private readonly int _min;
    private readonly int _max;
    
    public UniformDiscreteDistribution(int min = 0, int max = 1)
    {
        if (min >= max)
            throw new ArgumentOutOfRangeException(nameof(min), "Minimum value must be less than maximum value.");

        _min = min;
        _max = max;
    }
    
    public override double Expected => (_min + _max) / 2.0;
    
    public override double Mean => (_min + _max) / 2.0;
    
    public override double Median => (_min + _max) / 2.0;
    
    public override double Mode => double.NaN;
    
    public override double Variance => GetVariance();
    
    public override double Skewness => 0;
    
    public override double Kurtosis => -6.0 * (_max - _min + 1) * (_max - _min + 1) / (5.0 * (_max - _min) * (_max - _min + 2));
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => _min;
    
    public override double Maximum => _max;

    public int Range => _max - _min + 1;
    
    public int Size => _max - _min + 1;
    
    public double Entropy => Math.Log(_max - _min + 1);
    
    public override double Distribute() => RandomUtils.Next(_min, _max + 1);
    
    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return _min;
            case 1: return _max;
            default:
            {
                var index = (int)Math.Ceiling(p * (_max - _min + 1)) - 1 + _min;
                return Math.Min(Math.Max(index, _min), _max);
            }
        }
    }

    
    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < _min || k > _max || x != k)
            return 0;

        return 1.0 / (_max - _min + 1);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < _min)
            return 0;
            
        if (x >= _max)
            return 1;

        var floorX = (int)Math.Floor(x);
        return (floorX - _min + 1) / (double)(_max - _min + 1);
    }

    // Функция массы вероятности для дискретного равномерного распределения
    public double ProbabilityMass(int k)
    {
        if (k < _min || k > _max)
            return 0;

        return 1.0 / (_max - _min + 1);
    }
    
    public double Moment(int order)
    {
        if (order < 0)
            throw new ArgumentOutOfRangeException(nameof(order), "Moment order must be non-negative");

        if (order == 0)
            return 1;

        var moment = 0.0;
        for (var k = _min; k <= _max; k++)
        {
            moment += Math.Pow(k, order);
        }
        return moment / (_max - _min + 1);
    }

    public double CentralMoment(int order)
    {
        if (order < 0)
            throw new ArgumentOutOfRangeException(nameof(order), "Moment order must be non-negative");

        if (order == 0)
            return 1;

        var mean = Mean;
        var centralMoment = 0.0;
        for (var k = _min; k <= _max; k++)
        {
            centralMoment += Math.Pow(k - mean, order);
        }
        return centralMoment / (_max - _min + 1);
    }

    public override string ToString() => $"Uniform Discrete Distribution [Min = {_min}, Max = {_max}]";

    public override bool Equals(object obj) => obj is UniformDiscreteDistribution other && _min == other._min && _max == other._max;

    public override int GetHashCode() => HashCode.Combine(_min, _max);
    
    private double GetVariance()
    {
        var n = _max - _min + 1;
        return (n * n - 1) / 12.0;
    }
}