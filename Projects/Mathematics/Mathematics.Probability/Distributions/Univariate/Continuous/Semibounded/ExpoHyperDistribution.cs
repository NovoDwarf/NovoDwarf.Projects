using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

[Categories("Distributions", "Univariate", "Continious", "Semibounded")]
public partial class ExpoHyperDistribution : Distribution
{
    public override double Expected => GetMean();
    
    public override double Mean => GetMean();
    
    public override double Median => Quantile(0.5);
    
    public override double Mode
    {
        get
        {
            var maxDensity = 0.0;
            var mode = 0.0;
            
            for (var i = 0; i < Rates.Length; i++)
            {
                var density = _expoProbabilities[i] * Rates[i];
                
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
    
    [EntityParameter(typeof(double[]), nameof(Probabilities))]
    public double[] Probabilities { get; private set; } = [0.5, 0.5];
    
    [EntityParameter(typeof(double[]), nameof(Rates))]
    public double[] Rates { get; private set; } = [1.0, 0.1];
    
    private ExpoDistribution[] _components = [new(), new()];
    
    private double[] _expoProbabilities;
    
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

        return Rates.Select((t, i) => _expoProbabilities[i] * t * Math.Exp(-t * x)).Sum();
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;

        return Rates.Select((t, i) => _expoProbabilities[i] * (1 - Math.Exp(-t * x))).Sum();
    }
    
    public override string ToString()
    {
        var probsString = string.Join(", ", _expoProbabilities.Select(p => p.ToString("F3")));
        var ratesString = string.Join(", ", Rates.Select(r => r.ToString("F3")));
        
        return $"Expo Hyper Distribution [Probabilities = [{probsString}], Rates = [{ratesString}]]";
    }

    public override bool Equals(object? obj) => obj is ExpoHyperDistribution other && _expoProbabilities.SequenceEqual(other._expoProbabilities) && Rates.SequenceEqual(other.Rates);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        
        foreach (var prob in _expoProbabilities) 
            hash.Add(prob);
        
        foreach (var rate in Rates) 
            hash.Add(rate);
        
        return hash.ToHashCode();
    }

    protected override void Validate()
    {
        if (Probabilities.Length != Rates.Length)
            throw new ArgumentException("Probabilities and rates arrays must have the same length");
        
        if (Probabilities.Length == 0)
            throw new ArgumentException("Arrays cannot be empty");
        
        if (Probabilities.Any(probability => probability < 0))
            throw new ArgumentException("All probabilities must be non-negative", nameof(Probabilities));
        
        if (Rates.Any(rate => rate <= 0))
            throw new ArgumentException("All rates must be positive", nameof(Rates));
        
        var sum = Probabilities.Sum();
        
        if (sum <= 0)
            throw new ArgumentException("Sum of probabilities must be positive", nameof(Probabilities));
        
        _expoProbabilities = Probabilities.Select(p => p / sum).ToArray();
        _components = Rates.Select(r =>
        {
            var expo = new ExpoDistribution();

            expo.Set(r);
            
            return expo;
        }).ToArray();
    }

    private double GetMean()
    {
        return Rates.Select((t, i) => _expoProbabilities[i] / t).Sum();
    }

    private double GetVariance()
    {
        var mean = Mean;
        var secondMoment = Rates.Select((t, i) => _expoProbabilities[i] * 2.0 / (t * t)).Sum();

        return secondMoment - mean * mean;
    }

    private double GetSkewness()
    {
        var mean = Mean;
        var variance = Variance;
        var thirdMoment = Rates.Select((t, i) => _expoProbabilities[i] * 6.0 / (t * t * t)).Sum();

        var numerator = thirdMoment - 3 * mean * variance - mean * mean * mean;
        var denominator = Math.Pow(variance, 1.5);
        
        return numerator / denominator;
    }

    private double GetKurtosis()
    {
        var mean = Mean;
        var variance = Variance;
        var fourthMoment = Rates.Select((t, i) => _expoProbabilities[i] * 24.0 / (t * t * t * t)).Sum();

        var term1 = fourthMoment;
        var term2 = -4 * mean * (Skewness * Math.Pow(variance, 1.5) + 3 * mean * variance + mean * mean * mean);
        var term3 = 6 * mean * mean * (variance + mean * mean);
        var term4 = -3 * mean * mean * mean * mean;
        
        var numerator = term1 + term2 + term3 + term4;
        var denominator = variance * variance;
        
        return numerator / denominator;
    }
}