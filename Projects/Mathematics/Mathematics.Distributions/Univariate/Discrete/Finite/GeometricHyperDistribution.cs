using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Discrete.Finite;

public partial class GeometricHyperDistribution : Distribution
{
    private readonly int _populationSize;
    private readonly int _successStates;
    private readonly int _draws;
    
    public GeometricHyperDistribution(int populationSize = 20, int successStates = 5, int draws = 10)
    {
        if (populationSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(populationSize), "Population size must be positive.");

        if (successStates < 0 || successStates > populationSize)
            throw new ArgumentOutOfRangeException(nameof(successStates),
                "Success states must be between 0 and population size.");

        if (draws < 0 || draws > populationSize)
            throw new ArgumentOutOfRangeException(nameof(draws),
                "Number of draws must be between 0 and population size.");

        _populationSize = populationSize;
        _successStates = successStates;
        _draws = draws;
    }

    public override double Expected => _draws * _successStates / (double)_populationSize;
    
    public override double Mean => _draws * _successStates / (double)_populationSize;
    
    /// <summary>
    /// Медиана гипергеометрического распределения не имеет простой аналитической формы.
    /// Используем приближение floor((n+1)(K+1)/(N+2))
    /// </summary>
    public override double Median => Math.Floor((_draws + 1) * (_successStates + 1) / (double)(_populationSize + 2));

    /// <summary>
    /// Мода гипергеометрического распределения: floor((n+1)(K+1)/(N+2))
    /// </summary>
    public override double Mode => Math.Floor((_draws + 1) * (_successStates + 1) / (double)(_populationSize + 2));

    public override double Variance => GetVariance();
    
    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => Math.Max(0, _draws - (_populationSize - _successStates));
    
    public override double Maximum => Math.Min(_draws, _successStates);

    public int PopulationSize => _populationSize;
    
    public int SuccessStates => _successStates;
    
    public int Draws => _draws;
    
    public int N => _populationSize;
    
    public int K => _successStates;
    
    public int n => _draws;
    
    public override double Distribute()
    {
        var successes = 0;
        var remainingSuccesses = _successStates;
        var remainingPopulation = _populationSize;

        for (var i = 0; i < _draws; i++)
        {
            var probability = (double)remainingSuccesses / remainingPopulation;

            if (RandomUtils.NextDouble() < probability)
            {
                successes++;
                remainingSuccesses--;
            }

            remainingPopulation--;
        }

        return successes;
    }
    
    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return Minimum;
            case 1: return Maximum;
        }

        var cumulative = 0.0;
        
        for (var k = (int)Minimum; k <= Maximum; k++)
        {
            cumulative += ProbabilityMass(k);
            
            if (p <= cumulative)
                return k;
        }
        
        return Maximum;
    }

    public override double ProbabilityDensity(double x)
    {
        var k = (int)Math.Floor(x);
        
        if (k < Minimum || k > Maximum || x != k)
            return 0;

        return ProbabilityMass(k);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < Minimum)
            return 0;
            
        if (x >= Maximum)
            return 1;

        var floorX = (int)Math.Floor(x);
        var cdf = 0.0;
        
        for (var k = (int)Minimum; k <= floorX; k++)
        {
            cdf += ProbabilityMass(k);
        }
        
        return cdf;
    }

    // Функция массы вероятности для гипергеометрического распределения
    public double ProbabilityMass(int k)
    {
        if (k < Minimum || k > Maximum)
            return 0;

        // P(X = k) = [C(K, k) * C(N-K, n-k)] / C(N, n)
        var numerator = BinomialCoefficient(_successStates, k) * 
                       BinomialCoefficient(_populationSize - _successStates, _draws - k);
        var denominator = BinomialCoefficient(_populationSize, _draws);
        
        return numerator / denominator;
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
    
    public override string ToString() => $"Geometric Hyper Distribution [Population = {_populationSize}, Successes = {_successStates}, Draws = {_draws}]";

    public override bool Equals(object obj) => obj is GeometricHyperDistribution other && _populationSize == other._populationSize && _successStates == other._successStates && _draws == other._draws;

    public override int GetHashCode() => HashCode.Combine(_populationSize, _successStates, _draws);
    
    private double GetVariance()
    {
        var n = _draws;
        var K = _successStates;
        var N = _populationSize;
        return n * (K / (double)N) * ((N - K) / (double)N) * ((N - n) / (double)(N - 1));
    }
    
    private double GetSkewness()
    {
        var n = _draws;
        var K = _successStates;
        var N = _populationSize;
        var p = K / (double)N;
        var q = 1 - p;
            
        var numerator = (N - 2 * K) * Math.Sqrt(N - 1) * (N - 2 * n);
        var denominator = Math.Sqrt(n * K * (N - K) * (N - n)) * (N - 2);
            
        return numerator / denominator;
    }
    
    private double GetKurtosis()
    {
        var n = _draws;
        var K = _successStates;
        var N = _populationSize;
        var p = K / (double)N;
        var q = 1 - p;
            
        var term1 = (N - 1) * (N * N) * (N * (N + 1) - 6 * K * (N - K) - 6 * n * (N - n));
        var term2 = 6 * n * K * (N - K) * (N - n) * (5 * N - 6);
        var denominator = n * K * (N - K) * (N - n) * (N - 2) * (N - 3);
            
        return (term1 - term2) / denominator;
    }
}