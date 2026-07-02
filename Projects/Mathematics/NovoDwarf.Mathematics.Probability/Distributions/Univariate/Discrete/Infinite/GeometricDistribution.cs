using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Discrete.Infinite;

[Categories("Distributions", "Univariate", "Discrete", "Infinite")]
public partial class GeometricDistribution : Distribution
{
    public override double Expected => 1.0 / Probability;
    
    public override double Mean => 1.0 / Probability;
    
    public override double Median => Math.Ceiling(-Math.Log(2) / Math.Log(1 - Probability));
    
    public override double Mode => 1;
    
    public override double Variance => (1 - Probability) / (Probability * Probability);
    
    public override double Skewness => (2 - Probability) / Math.Sqrt(1 - Probability);
    
    public override double Kurtosis => 6 + (Probability * Probability) / (1 - Probability);
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 1;
    
    public override double Maximum => double.PositiveInfinity;

    [EntityParameter(typeof(double), nameof(Probability))]
    public double Probability { get; set; }

    public double Q => 1 - Probability;
    
    public override double Distribute() => Math.Floor(Math.Log(1 - RandomUtils.NextDouble()) / Math.Log(1 - Probability)) + 1;

    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        return p switch
        {
            0 => 1,
            1 => double.PositiveInfinity,
            _ => Math.Ceiling(Math.Log(1 - p) / Math.Log(1 - Probability))
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
        return 1 - Math.Pow(1 - Probability, floorX);
    }

    public double ProbabilityMass(int k)
    {
        if (k < 1)
            return 0;

        return Probability * Math.Pow(1 - Probability, k - 1);
    }

    public override string ToString() => $"Geometric Distribution [Probability = {Probability}]";

    public override bool Equals(object? obj) => obj is GeometricDistribution other && Probability == other.Probability;

    public override int GetHashCode() => Probability.GetHashCode();

    protected override void Validate()
    {
        
    }
}