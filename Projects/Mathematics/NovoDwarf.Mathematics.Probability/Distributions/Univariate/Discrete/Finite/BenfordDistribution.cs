using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Discrete.Finite;

[Categories("Distributions", "Univariate", "Discrete", "Finite")]
public partial class BenfordDistribution : Distribution
{
    public override double Expected => GetMean();
    
    public override double Mean => GetMean();
    
    public override double Median => 3;
    
    public override double Mode => 1;
    
    public override double Variance => GetVariance();
    
    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 1;
    
    public override double Maximum => 9;

    public override double Distribute()
    {
        var randomValue = RandomUtils.NextDouble();
        var cumulativeProbability = 0.0;

        for (var digit = 1; digit <= 9; digit++)
        {
            cumulativeProbability += GetProbabilityForDigit(digit);

            if (randomValue <= cumulativeProbability)
                return digit;
        }

        return 9;
    }

    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        if (p == 0) return 1;
        if (p == 1) return 9;

        var cumulative = 0.0;
        
        for (var digit = 1; digit <= 9; digit++)
        {
            cumulative += GetProbabilityForDigit(digit);
           
            if (p <= cumulative)
                return digit;
        }
        
        return 9;
    }
    
    public override double ProbabilityDensity(double x)
    {
        var digit = (int)Math.Floor(x);
        
        if (digit < 1 || digit > 9)
            return 0;
            
        if (x != digit)
            return 0;

        return GetProbabilityForDigit(digit);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 1)
            return 0;
            
        if (x >= 9)
            return 1;

        var floorX = (int)Math.Floor(x);
        var cdf = 0.0;
        
        for (var digit = 1; digit <= floorX; digit++)
        {
            cdf += GetProbabilityForDigit(digit);
        }
        
        return cdf;
    }

    private static double GetProbabilityForDigit(int digit)
    {
        return digit is < 1 or > 9 
            ? throw new ArgumentOutOfRangeException(nameof(digit), "Digit must be between 1 and 9.") 
            : Math.Log10(1.0 + 1.0 / digit);
    }
    
    public override string ToString() => "Benford Distribution";

    public override bool Equals(object? obj) => obj is BenfordDistribution;

    public override int GetHashCode() => typeof(BenfordDistribution).GetHashCode();

    protected override void Validate()
    {
        
    }

    private double GetMean()
    {
        var mean = 0.0;
        
        for (var digit = 1; digit <= 9; digit++) 
            mean += digit * GetProbabilityForDigit(digit);
        
        return mean;
    }

    private double GetVariance()
    {
        var mean = Mean;
        var secondMoment = 0.0;
        
        for (var digit = 1; digit <= 9; digit++) 
            secondMoment += digit * digit * GetProbabilityForDigit(digit);
        
        return secondMoment - mean * mean;
    }

    private double GetSkewness()
    {
        var mean = Mean;
        var variance = Variance;
        var thirdMoment = 0.0;
        
        for (var digit = 1; digit <= 9; digit++) 
            thirdMoment += Math.Pow(digit - mean, 3) * GetProbabilityForDigit(digit);
        
        return thirdMoment / Math.Pow(variance, 1.5);
    }

    private double GetKurtosis()
    {
        var mean = Mean;
        var variance = Variance;
        var fourthMoment = 0.0;
        
        for (var digit = 1; digit <= 9; digit++) 
            fourthMoment += Math.Pow(digit - mean, 4) * GetProbabilityForDigit(digit);
        
        return fourthMoment / (variance * variance) - 3;
    }
}