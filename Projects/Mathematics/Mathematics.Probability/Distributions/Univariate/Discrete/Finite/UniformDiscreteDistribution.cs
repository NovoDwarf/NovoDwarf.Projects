using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Discrete.Finite;

[Categories("Distributions", "Univariate", "Discrete", "Finite")]
public partial class UniformDiscreteDistribution : Distribution
{
    public override double Expected => (_minimum + _maximum) / 2.0;
    
    public override double Mean => (_minimum + _maximum) / 2.0;
    
    public override double Median => (_minimum + _maximum) / 2.0;
    
    public override double Mode => double.NaN;
    
    public override double Variance => GetVariance();
    
    public override double Skewness => 0;
    
    public override double Kurtosis => -6.0 * (_maximum - _minimum + 1) * (_maximum - _minimum + 1) / (5.0 * (_maximum - _minimum) * (_maximum - _minimum + 2));
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => _minimum;
    
    public override double Maximum => _maximum;
    
    public int Range => _maximum - _minimum + 1;
    
    public int Size => _maximum - _minimum + 1;
    
    public double Entropy => Math.Log(_maximum - _minimum + 1);
    
    [EntityParameter(typeof(double), nameof(Minimum))]
    private int _minimum = 0;
   
    [EntityParameter(typeof(double), nameof(Maximum))]
    private int _maximum = 1;

    protected override void Validate()
    {
        if (Minimum >= Maximum)
            throw new ArgumentOutOfRangeException(nameof(Minimum), "Minimum value must be less than maximum value.");
    }

    public override double Distribute() => RandomUtils.Next(_minimum, _maximum + 1);
    
    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return _minimum;
            case 1: return _maximum;
            default:
            {
                var index = (int)Math.Ceiling(p * (_maximum - _minimum + 1)) - 1 + _minimum;
                return Math.Min(Math.Max(index, _minimum), _maximum);
            }
        }
    }

    
    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < _minimum || k > _maximum || x != k)
            return 0;

        return 1.0 / (_maximum - _minimum + 1);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < _minimum)
            return 0;
            
        if (x >= _maximum)
            return 1;

        var floorX = (int)Math.Floor(x);
        return (floorX - _minimum + 1) / (double)(_maximum - _minimum + 1);
    }
    
    public override string ToString() => $"Uniform Discrete Distribution [Min = {_minimum}, Max = {_maximum}]";

    public override bool Equals(object? obj) => obj is UniformDiscreteDistribution other && _minimum == other._minimum && _maximum == other._maximum;

    public override int GetHashCode() => HashCode.Combine(_minimum, _maximum);
    
    private double GetVariance()
    {
        var n = _maximum - _minimum + 1;
        return (n * n - 1) / 12.0;
    }
}