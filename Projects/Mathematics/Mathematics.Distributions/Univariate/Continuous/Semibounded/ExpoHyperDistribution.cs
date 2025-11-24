using Mathematics.Core.Base;
using Mathematics.Core.Utilities;

namespace Mathematics.Distributions.Univariate.Continuous.Semibounded;

public partial class ExpoHyperDistribution : Distribution
{
    private readonly ExpoDistribution[] _components;
    private readonly double[] _expoProbabilities;

    private readonly double[] _probabilities;
    private readonly double[] _rates;
    
    public ExpoHyperDistribution(double[]? probabilities = null, double[]? rates = null)
    {
        _probabilities = probabilities ?? [0.5, 0.5];
        _rates = rates ?? [1.0, 0.1];

        if (_probabilities.Length != _rates.Length)
            throw new ArgumentException("Probabilities and rates arrays must have the same length");
        
        if (_probabilities.Length == 0)
            throw new ArgumentException("Arrays cannot be empty");
        
        if (_probabilities.Any(probability => probability < 0))
            throw new ArgumentException("All probabilities must be non-negative", nameof(probabilities));
        
        if (_rates.Any(rate => rate <= 0))
            throw new ArgumentException("All rates must be positive", nameof(rates));

        var sum = _probabilities.Sum();
        
        if (sum <= 0)
            throw new ArgumentException("Sum of probabilities must be positive", nameof(probabilities));

        _expoProbabilities = _probabilities.Select(p => p / sum).ToArray();
        _components = _rates.Select(r => new ExpoDistribution(r)).ToArray();
    }

    public override double Expected => GetMean();
    
    public override double Mean => GetMean();
    
    public override double Median => Quantile(0.5);
    
    public override double Mode
    {
        get
        {
            var maxDensity = 0.0;
            var mode = 0.0;
            
            for (var i = 0; i < _rates.Length; i++)
            {
                var density = _expoProbabilities[i] * _rates[i];
                
                if (!(density > maxDensity)) 
                    continue;
                
                maxDensity = density;
                mode = 0;
            }
            
            return mode;
        }
    }
    
    public override double Variance => GetVariance();
    
    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => double.PositiveInfinity;

    public double CoefficientOfVariation => Math.Sqrt(Variance) / Mean;

    public double[] Probabilities => _expoProbabilities.ToArray();
    
    public double[] Rates => _rates.ToArray();
    
    public int ComponentCount => _rates.Length;
    
    public override double Distribute()
    {
        var r = RandomUtils.NextDouble();
        var cumulative = 0.0;
        var index = _expoProbabilities.Length - 1;

        for (var i = 0; i < _expoProbabilities.Length; i++)
        {
            cumulative += _expoProbabilities[i];

            if (r < cumulative || i == _expoProbabilities.Length - 1)
            {
                index = i;
                break;
            }
        }

        return _components[index].Distribute();
    }
    
    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return 0;
            case 1: return double.PositiveInfinity;
        }

        double low = 0;
        double high = 1;
        
        while (CumulativeDistribution(high) < p)
        {
            high *= 2;
        }

        const double tolerance = 1e-10;
        const int maxIterations = 100;

        for (var i = 0; i < maxIterations; i++)
        {
            var mid = (low + high) / 2;
            var fmid = CumulativeDistribution(mid);

            if (Math.Abs(fmid - p) < tolerance)
                return mid;

            if (fmid < p)
                low = mid;
            else
                high = mid;
        }

        return (low + high) / 2;
    }

    public override double ProbabilityDensity(double x)
    {
        if (x < 0)
            return 0;

        return _rates.Select((t, i) => _expoProbabilities[i] * t * Math.Exp(-t * x)).Sum();
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;

        return _rates.Select((t, i) => _expoProbabilities[i] * (1 - Math.Exp(-t * x))).Sum();
    }
    
    public override string ToString()
    {
        var probsString = string.Join(", ", _expoProbabilities.Select(p => p.ToString("F3")));
        var ratesString = string.Join(", ", _rates.Select(r => r.ToString("F3")));
        return $"Expo Hyper Distribution [Probabilities = [{probsString}], Rates = [{ratesString}]]";
    }

    public override bool Equals(object? obj) => obj is ExpoHyperDistribution other && _expoProbabilities.SequenceEqual(other._expoProbabilities) && _rates.SequenceEqual(other._rates);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        
        foreach (var prob in _expoProbabilities) 
            hash.Add(prob);
        
        foreach (var rate in _rates) 
            hash.Add(rate);
        
        return hash.ToHashCode();
    }
    
    private double GetMean()
    {
        return _rates.Select((t, i) => _expoProbabilities[i] / t).Sum();
    }

    private double GetVariance()
    {
        var mean = Mean;
        var secondMoment = _rates.Select((t, i) => _expoProbabilities[i] * 2.0 / (t * t)).Sum();

        return secondMoment - mean * mean;
    }

    private double GetSkewness()
    {
        var mean = Mean;
        var variance = Variance;
        var thirdMoment = _rates.Select((t, i) => _expoProbabilities[i] * 6.0 / (t * t * t)).Sum();

        var numerator = thirdMoment - 3 * mean * variance - mean * mean * mean;
        var denominator = Math.Pow(variance, 1.5);
        
        return numerator / denominator;
    }

    private double GetKurtosis()
    {
        var mean = Mean;
        var variance = Variance;
        var fourthMoment = _rates.Select((t, i) => _expoProbabilities[i] * 24.0 / (t * t * t * t)).Sum();

        var term1 = fourthMoment;
        var term2 = -4 * mean * (Skewness * Math.Pow(variance, 1.5) + 3 * mean * variance + mean * mean * mean);
        var term3 = 6 * mean * mean * (variance + mean * mean);
        var term4 = -3 * mean * mean * mean * mean;
        
        var numerator = term1 + term2 + term3 + term4;
        var denominator = variance * variance;
        
        return numerator / denominator;
    }
}