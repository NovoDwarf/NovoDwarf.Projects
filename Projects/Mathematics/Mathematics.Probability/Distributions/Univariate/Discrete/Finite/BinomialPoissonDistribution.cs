using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Discrete.Finite;

[Categories("Distributions", "Univariate", "Discrete", "Finite")]
public partial class BinomialPoissonDistribution : Distribution
{
    public override double Expected => Success.Sum();
    
    public override double Mean => Success.Sum();

    public override double Median => Quantile(0.5);

    public override double Mode => GetMode();

    public override double Variance => Success.Sum(t => t * (1 - t));

    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => Success.Length;

    public override double Distribute() => SamplePoissonBinomial(Success);

    [EntityParameter(typeof(double[]), nameof(Success))]
    public double[] Success { get; private set; } = [0.3, 0.7];
    
    protected override void Validate()
    {
        if (Success == null || Success.Length == 0)
            throw new ArgumentException("Success probabilities array cannot be null or empty", nameof(Success));
        
        if (Success.Any(prob => prob is < 0 or > 1))
            throw new ArgumentOutOfRangeException(nameof(Success), "All probabilities must be between 0 and 1");
    }

    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return 0;
            case 1: return Success.Length;
        }
        
        var cumulative = 0.0;
        
        for (var k = 0; k <= Success.Length; k++)
        {
            cumulative += ProbabilityMass(k);
            
            if (p <= cumulative)
                return k;
        }
        
        return Success.Length;
    }
    
    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < 0 || k > Success.Length || x != k)
            return 0;

        return ProbabilityMass(k);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;
            
        if (x >= Success.Length)
            return 1;

        var floorX = (int)Math.Floor(x);
        var cdf = 0.0;
        
        for (var k = 0; k <= floorX; k++)
        {
            cdf += ProbabilityMass(k);
        }
        
        return cdf;
    }

    private int SamplePoissonBinomial(double[] probabilities)
    {
        if (probabilities.Length == 0)
            return 0;

        return probabilities.Count(p => RandomUtils.NextDouble() < p);
    }

    public double ProbabilityMass(int k)
    {
        if (k < 0 || k > Success.Length)
            return 0;

        var n = Success.Length;
        var dp = new double[n + 1];
        dp[0] = 1.0;
        
        for (var i = 0; i < n; i++)
        {
            var p = Success[i];
            var q = 1 - p;
            
            for (var j = i + 1; j >= 1; j--) 
                dp[j] = dp[j] * q + dp[j - 1] * p;
            
            dp[0] *= q;
        }
        
        return dp[k];
    }
    
    public double[] SuccessProbabilities => (double[])Success.Clone();
    public int TrialCount => Success.Length;

    public override string ToString()
    {
        var probsString = string.Join(", ", Success.Select(p => p.ToString("F3")));
        
        return $"Binomial Poisson Distribution [Probabilities = [{probsString}]]";
    }

    public override bool Equals(object? obj) => obj is BinomialPoissonDistribution other && Success.SequenceEqual(other.Success);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        
        foreach (var prob in Success)
        {
            hash.Add(prob);
        }
        
        return hash.ToHashCode();
    }
    
    private double GetMode()
    {
        var mean = Mean;
        var floor = Math.Floor(mean);
        var ceil = Math.Ceiling(mean);
            
        var probFloor = ProbabilityMass((int)floor);
        var probCeil = ProbabilityMass((int)ceil);
            
        return probFloor >= probCeil ? floor : ceil;
    }
    
    private double GetSkewness()
    {
        var variance = Variance;
            
        if (variance == 0)
            return double.NaN;
                
        var thirdCentralMoment = Success.Sum(p => p * (1 - p) * (1 - 2 * p));

        return thirdCentralMoment / Math.Pow(variance, 1.5);
    }

    private double GetKurtosis()
    {
        var variance = Variance;
            
        if (variance == 0)
            return double.NaN;
                
        var fourthCentralMoment = Success.Sum(p => p * (1 - p) * (1 - 6 * p * (1 - p)));

        return fourthCentralMoment / (variance * variance) - 3;
    }
}

public struct Complex
{
    public double Real { get; }
    public double Imaginary { get; }
    
    public Complex(double real, double imaginary = 0)
    {
        Real = real;
        Imaginary = imaginary;
    }
    
    public static implicit operator Complex(double real) => new Complex(real);
}