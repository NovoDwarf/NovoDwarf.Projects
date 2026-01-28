using System.ComponentModel.DataAnnotations;
using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

[Categories("Distributions", "Univariate", "Continious", "Semibounded")]
public partial class ExpoDistribution : Distribution
{
    public override double Expected => 1.0 / Rate;
    
    public override double Mean => 1.0 / Rate;
    
    public override double Median => Math.Log(2) / Rate;
    
    public override double Mode => 0;
    
    public override double Variance => 1.0 / (Rate * Rate);
    
    public override double Skewness => 2;
    
    public override double Kurtosis => 6;
    
    public override double StandardDeviation => 1.0 / Rate;
    
    public override double Minimum => 0;
    
    public override double Maximum => double.PositiveInfinity;

    [EntityParameter(typeof(double), nameof(Rate))]
    [Range(0, double.PositiveInfinity)]
    public double Rate { get; private set; } = 1;

    public double Scale => 1 / Rate;
    
    public override double Distribute()
    {
        var u = RandomUtils.NextDoubleSafe();
        
        return -Math.Log(u) / Rate;
    }
    
    public override double Quantile(double p)
    {
        if (p < 0 || p > 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        return p switch
        {
            0 => 0,
            1 => double.PositiveInfinity,
            _ => -Math.Log(1 - p) / Rate
        };
    }

    public override double ProbabilityDensity(double x)
    {
        if (x < 0)
            return 0;

        return Rate * Math.Exp(-Rate * x);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;

        return 1 - Math.Exp(-Rate * x);
    }
    
    public override string ToString() => $"Expo Distribution [Rate = {Rate}]";

    public override bool Equals(object? obj) => obj is ExpoDistribution other && Rate == other.Rate;

    public override int GetHashCode() => Rate.GetHashCode();

    protected override void Validate()
    {
        
    }
}