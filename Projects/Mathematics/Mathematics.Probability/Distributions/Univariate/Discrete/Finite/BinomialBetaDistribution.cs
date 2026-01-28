using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;
using Mathematics.Numerical.Simple;
using Mathematics.Probability.Distributions.Univariate.Continuous.Bounded;

namespace Mathematics.Probability.Distributions.Univariate.Discrete.Finite;

[Categories("Distributions", "Univariate", "Discrete", "Finite")]
public partial class BinomialBetaDistribution : Distribution
{
    public override double Expected => Trials * (Alpha / (Alpha + Beta));
    
    public override double Mean => Trials * (Alpha / (Alpha + Beta));
    
    public override double Median => Quantile(0.5);

    public override double Mode => GetMode();
    
    public override double Variance => GetVariance();

    public override double Skewness => GetSkewness();

    public override double Kurtosis => GetKurtosis();

    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => Trials;

    [EntityParameter(typeof(double), nameof(Alpha))]
    public double Alpha { get; private set; } = 1;

    [EntityParameter(typeof(double), nameof(Beta))]
    public double Beta { get; private set; } = 1;

    [EntityParameter(typeof(int), nameof(Trials))]
    public int Trials { get; private set; } = 10;

    private BetaDistribution _betaDist = new();
    
    protected override void Validate()
    {
        if (Alpha <= 0)
            throw new ArgumentOutOfRangeException(nameof(Alpha), "Alpha must be positive.");
        if (Beta <= 0)
            throw new ArgumentOutOfRangeException(nameof(Beta), "Beta must be positive.");
        if (Trials < 0)
            throw new ArgumentOutOfRangeException(nameof(Trials), "Number of trials must be non-negative.");
    }

    public override double Distribute()
    {
        var p = _betaDist.Distribute();
        return SampleBinomial(Trials, p);
    }
    
    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        if (p == 0) 
            return 0;
       
        if (p == 1) 
            return Trials;
        
        var cumulative = 0.0;
        
        for (var k = 0; k <= Trials; k++)
        {
            cumulative += ProbabilityMass(k);
            
            if (p <= cumulative)
                return k;
        }
        
        return Trials;
    }

    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < 0 || k > Trials || x != k)
            return 0;

        return ProbabilityMass(k);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;
            
        if (x >= Trials)
            return 1;

        var floorX = (int)Math.Floor(x);
        var cdf = 0.0;
        
        for (var k = 0; k <= floorX; k++)
        {
            cdf += ProbabilityMass(k);
        }
        
        return cdf;
    }

    private int SampleBinomial(int n, double p)
    {
        if (n == 0 || p == 0)
            return 0;

        if (p == 1)
            return n;

        var successes = 0;

        for (var i = 0; i < n; i++)
            if (RandomUtils.NextDouble() < p)
                successes++;
        return successes;
    }

    public double ProbabilityMass(int k)
    {
        if (k < 0 || k > Trials)
            return 0;

        var binomialCoeff = BinomialCoefficient(Trials, k);
        var betaNumerator = BetaFunction.Calculate(k + Alpha, Trials - k + Beta);
        var betaDenominator = BetaFunction.Calculate(Alpha, Beta);
        
        return binomialCoeff * betaNumerator / betaDenominator;
    }

    private static double BinomialCoefficient(int n, int k)
    {
        if (k < 0 || k > n)
            return 0;
            
        if (k == 0 || k == n)
            return 1;

        k = Math.Min(k, n - k);
        double result = 1;
        
        for (var i = 1; i <= k; i++)
        {
            result *= (n - k + i) / (double)i;
        }
        
        return result;
    }
    
    public override string ToString() => $"Binomial Beta Distribution [Alpha = {Alpha}, Beta = {Beta}, Trials = {Trials}]";

    public override bool Equals(object? obj) => obj is BinomialBetaDistribution other && Alpha == other.Alpha && Beta == other.Beta && Trials == other.Trials;

    public override int GetHashCode() => HashCode.Combine(Alpha, Beta, Trials);

    private double GetMode()
    {
        if (Trials == 0)
            return 0;

        var numerator = (Trials - 1) * (Alpha - 1);
        var denominator = Alpha + Beta - 2;
            
        if (denominator <= 0)
            return double.NaN;
            
        var mode = numerator / denominator;
            

        if (mode < 0)
            return 0;
            
        if (mode > Trials)
            return Trials;
            
        return Math.Round(mode);
    }
    
    private double GetVariance()
    {
        var n = Trials;
        var alphaBeta = Alpha + Beta;
        return n * Alpha * Beta * (alphaBeta + n) / (alphaBeta * alphaBeta * (alphaBeta + 1));
    }
    
    private double GetSkewness()
    {
        var n = Trials;
        var alphaBeta = Alpha + Beta;
        var numerator = (alphaBeta + 2 * n) * (Beta - Alpha) / (alphaBeta + 2);
        var denominator = Math.Sqrt(Alpha * Beta * (alphaBeta + n) * (alphaBeta + 2) / (n * (alphaBeta + 1)));
            
        return numerator / denominator;
    }
    
    private double GetKurtosis()
    {
        var n = Trials;
        var alphaBeta = Alpha + Beta;
        var term1 = (Alpha - Beta) / (Alpha + Beta);
        var term2 = (alphaBeta * (alphaBeta - 1 + 6 * n)) + 3 * Alpha * Beta * (n - 2) + 6 * n * n;
        var term3 = 3 * Alpha * Beta * n * (6 - n) / (alphaBeta * alphaBeta);
        var term4 = 18 * Alpha * Beta * n * n / (alphaBeta * alphaBeta * alphaBeta);
            
        var numerator = term2 - term3 + term4 - 3 * alphaBeta * alphaBeta;
        var denominator = Alpha * Beta * (alphaBeta + 2) * (alphaBeta + 3) * (alphaBeta + n);
            
        return numerator / denominator;
    }
}