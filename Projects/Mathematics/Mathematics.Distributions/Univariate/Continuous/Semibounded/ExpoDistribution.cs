using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Continuous.Semibounded;

public partial class ExpoDistribution : Distribution
{
    private readonly double _rate;
    
    public ExpoDistribution(double rate = 1)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(rate, 0);

        _rate = rate;
    }

    public override double Expected => 1.0 / _rate;
    
    public override double Mean => 1.0 / _rate;
    
    public override double Median => Math.Log(2) / _rate;
    
    public override double Mode => 0;
    
    public override double Variance => 1.0 / (_rate * _rate);
    
    public override double Skewness => 2;
    
    public override double Kurtosis => 6;
    
    public override double StandardDeviation => 1.0 / _rate;
    
    public override double Minimum => 0;
    
    public override double Maximum => double.PositiveInfinity;

    public double Rate => _rate;
    
    public override double Distribute()
    {
        var u = RandomUtils.NextDoubleSafe();
        
        return -Math.Log(u) / _rate;
    }
    
    public override double Quantile(double p)
    {
        if (p < 0 || p > 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        return p switch
        {
            0 => 0,
            1 => double.PositiveInfinity,
            _ => -Math.Log(1 - p) / _rate
        };
    }

    public override double ProbabilityDensity(double x)
    {
        if (x < 0)
            return 0;

        return _rate * Math.Exp(-_rate * x);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;

        return 1 - Math.Exp(-_rate * x);
    }
    
    public override string ToString() => $"Expo Distribution [Rate = {_rate}]";

    public override bool Equals(object? obj) => obj is ExpoDistribution other && _rate == other._rate;

    public override int GetHashCode() => _rate.GetHashCode();
}