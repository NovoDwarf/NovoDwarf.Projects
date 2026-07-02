using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Discrete.Finite;

[Categories("Distributions", "Univariate", "Discrete", "Finite")]
public partial class BernoulliDistribution : Distribution
{
    public override double Expected => Probability;
    
    public override double Mean => Probability;
    
    public override double Median => Probability switch
        {
            < 0.5 => 0,
            > 0.5 => 1,
            _ => 0.5
        };

    public override double Mode => Probability switch
        {
            > 0.5 => 1,
            < 0.5 => 0,
            _ => double.NaN
        };

    public override double Variance => Probability * (1 - Probability);
    
    public override double Skewness => Probability is 0 or 1
            ? double.NaN
            : (1 - 2 * Probability) / Math.Sqrt(Probability * (1 - Probability));

    public override double Kurtosis => Probability is 0 or 1
            ? double.NaN
            : (1 - 6 * Probability * (1 - Probability)) / (Probability * (1 - Probability));

    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => 1;

    [EntityParameter(typeof(double), nameof(Probability))]
    public double Probability { get; private set; } = 0.5;
    
    public double Q => 1 - Probability;

    protected override void Validate()
    {
        if (Probability is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(Probability), "Probability must be between 0 and 1.");
    }
    
    public override double Distribute() => RandomUtils.NextDouble() < Probability ? 1.0 : 0.0;

    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        return p <= 1 - Probability ? 0 : 1;
    }

    
    public override double ProbabilityDensity(double x)
    {
        return x switch
        {
            0 => 1 - Probability,
            1 => Probability,
            _ => 0
        };
    }

    public override double CumulativeDistribution(double x)
    {
        return x switch
        {
            < 0 => 0,
            < 1 => 1 - Probability,
            _ => 1
        };
    }

    public override string ToString() => $"Bernoulli Distribution [Probability = {Probability}]";

    public override bool Equals(object? obj) => obj is BernoulliDistribution other && Probability == other.Probability;

    public override int GetHashCode() => Probability.GetHashCode();
}