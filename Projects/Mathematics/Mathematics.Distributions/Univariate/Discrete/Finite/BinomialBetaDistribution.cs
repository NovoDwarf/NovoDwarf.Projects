using Mathematics.Core.Base;
using Mathematics.Core.Utilities;
using Mathematics.Functions;

namespace Mathematics.Distributions.Univariate.Discrete.Finite;

public partial class BinomialBetaDistribution : Distribution
{
    private readonly Continuous.Bounded.BetaDistribution _betaDist;
    
    private readonly double _alpha;
    private readonly double _beta;
    private readonly int _trials;
    
    public BinomialBetaDistribution(double alpha = 1, double beta = 1, int trials = 10)
    {
        if (alpha <= 0)
            throw new ArgumentOutOfRangeException(nameof(alpha), "Alpha must be positive.");
        if (beta <= 0)
            throw new ArgumentOutOfRangeException(nameof(beta), "Beta must be positive.");
        if (trials < 0)
            throw new ArgumentOutOfRangeException(nameof(trials), "Number of trials must be non-negative.");

        _alpha = alpha;
        _beta = beta;
        _trials = trials;
        _betaDist = new Continuous.Bounded.BetaDistribution(alpha, beta, 1);
    }
    
    public override double Expected => _trials * (_alpha / (_alpha + _beta));
    
    public override double Mean => _trials * (_alpha / (_alpha + _beta));
    
    public override double Median => Quantile(0.5);

    public override double Mode => GetMode();


    public override double Variance => GetVariance();

    public override double Skewness => GetSkewness();

    public override double Kurtosis => GetKurtosis();

    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => _trials;

    public double Alpha => _alpha;
    
    public double Beta => _beta;
    
    public int Trials => _trials;
    
    public override double Distribute()
    {
        var p = _betaDist.Distribute();
        return SampleBinomial(_trials, p);
    }
    
    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        if (p == 0) 
            return 0;
       
        if (p == 1) 
            return _trials;
        
        var cumulative = 0.0;
        
        for (var k = 0; k <= _trials; k++)
        {
            cumulative += ProbabilityMass(k);
            
            if (p <= cumulative)
                return k;
        }
        
        return _trials;
    }

    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < 0 || k > _trials || x != k)
            return 0;

        return ProbabilityMass(k);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;
            
        if (x >= _trials)
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
        if (k < 0 || k > _trials)
            return 0;

        var binomialCoeff = BinomialCoefficient(_trials, k);
        var betaNumerator = BetaFunction.Calculate(k + _alpha, _trials - k + _beta);
        var betaDenominator = BetaFunction.Calculate(_alpha, _beta);
        
        return binomialCoeff * betaNumerator / betaDenominator;
    }

    // Вспомогательные математические функции
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
    
    public override string ToString() => $"Binomial Beta Distribution [Alpha = {_alpha}, Beta = {_beta}, Trials = {_trials}]";

    public override bool Equals(object obj) => obj is BinomialBetaDistribution other && _alpha == other._alpha && _beta == other._beta && _trials == other._trials;

    public override int GetHashCode() => HashCode.Combine(_alpha, _beta, _trials);

    private double GetMode()
    {
        if (_trials == 0)
            return 0;

        var numerator = (_trials - 1) * (_alpha - 1);
        var denominator = _alpha + _beta - 2;
            
        if (denominator <= 0)
            return double.NaN;
            
        var mode = numerator / denominator;
            

        if (mode < 0)
            return 0;
            
        if (mode > _trials)
            return _trials;
            
        return Math.Round(mode);
    }
    
    private double GetVariance()
    {
        var n = _trials;
        var alphaBeta = _alpha + _beta;
        return n * _alpha * _beta * (alphaBeta + n) / (alphaBeta * alphaBeta * (alphaBeta + 1));
    }
    
    private double GetSkewness()
    {
        var n = _trials;
        var alphaBeta = _alpha + _beta;
        var numerator = (alphaBeta + 2 * n) * (_beta - _alpha) / (alphaBeta + 2);
        var denominator = Math.Sqrt(_alpha * _beta * (alphaBeta + n) * (alphaBeta + 2) / (n * (alphaBeta + 1)));
            
        return numerator / denominator;
    }
    
    private double GetKurtosis()
    {
        var n = _trials;
        var alphaBeta = _alpha + _beta;
        var term1 = (_alpha - _beta) / (_alpha + _beta);
        var term2 = (alphaBeta * (alphaBeta - 1 + 6 * n)) + 3 * _alpha * _beta * (n - 2) + 6 * n * n;
        var term3 = 3 * _alpha * _beta * n * (6 - n) / (alphaBeta * alphaBeta);
        var term4 = 18 * _alpha * _beta * n * n / (alphaBeta * alphaBeta * alphaBeta);
            
        var numerator = term2 - term3 + term4 - 3 * alphaBeta * alphaBeta;
        var denominator = _alpha * _beta * (alphaBeta + 2) * (alphaBeta + 3) * (alphaBeta + n);
            
        return numerator / denominator;
    }
}