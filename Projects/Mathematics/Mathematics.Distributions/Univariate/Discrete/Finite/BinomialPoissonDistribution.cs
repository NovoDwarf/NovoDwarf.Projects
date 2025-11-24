using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Discrete.Finite;

public partial class BinomialPoissonDistribution : Distribution
{
    private readonly double[] _success;

    public BinomialPoissonDistribution(double[]? success = null)
    {
        success ??= [0.3, 0.7];
        
        if (success == null || success.Length == 0)
            throw new ArgumentException("Success probabilities array cannot be null or empty", nameof(success));
        
        if (success.Any(prob => prob is < 0 or > 1))
            throw new ArgumentOutOfRangeException(nameof(success), "All probabilities must be between 0 and 1");

        _success = (double[])success.Clone();
    }
    
    public override double Expected => _success.Sum();
    
    public override double Mean => _success.Sum();
    
    /// <summary>
    /// Медиана распределения Пуассона-Биномиала не имеет простой аналитической формы
    /// Используем приближение через квантиль
    /// </summary>
    public override double Median => Quantile(0.5);

    public override double Mode => GetMode();



    public override double Variance => _success.Sum(t => t * (1 - t));

    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => _success.Length;

    public override double Distribute() => SamplePoissonBinomial(_success);

    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return 0;
            case 1: return _success.Length;
        }
        
        var cumulative = 0.0;
        
        for (var k = 0; k <= _success.Length; k++)
        {
            cumulative += ProbabilityMass(k);
            
            if (p <= cumulative)
                return k;
        }
        
        return _success.Length;
    }
    
    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < 0 || k > _success.Length || x != k)
            return 0;

        return ProbabilityMass(k);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;
            
        if (x >= _success.Length)
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

    // Функция массы вероятности для распределения Пуассона-Биномиала
    public double ProbabilityMass(int k)
    {
        if (k < 0 || k > _success.Length)
            return 0;

        var n = _success.Length;
        var dp = new double[n + 1];
        dp[0] = 1.0;
        
        for (var i = 0; i < n; i++)
        {
            var p = _success[i];
            var q = 1 - p;
            
            for (var j = i + 1; j >= 1; j--) 
                dp[j] = dp[j] * q + dp[j - 1] * p;
            
            dp[0] *= q;
        }
        
        return dp[k];
    }
    
    public double[] SuccessProbabilities => (double[])_success.Clone();
    public int TrialCount => _success.Length;

    public override string ToString()
    {
        var probsString = string.Join(", ", _success.Select(p => p.ToString("F3")));
        
        return $"Binomial Poisson Distribution [Probabilities = [{probsString}]]";
    }

    public override bool Equals(object obj) => obj is BinomialPoissonDistribution other && _success.SequenceEqual(other._success);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        
        foreach (var prob in _success)
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
                
        var thirdCentralMoment = _success.Sum(p => p * (1 - p) * (1 - 2 * p));

        return thirdCentralMoment / Math.Pow(variance, 1.5);
    }

    private double GetKurtosis()
    {
        var variance = Variance;
            
        if (variance == 0)
            return double.NaN;
                
        var fourthCentralMoment = _success.Sum(p => p * (1 - p) * (1 - 6 * p * (1 - p)));

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