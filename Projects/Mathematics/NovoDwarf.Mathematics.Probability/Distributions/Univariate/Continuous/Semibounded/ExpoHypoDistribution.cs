using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

[Categories("Distributions", "Univariate", "Continious", "Semibounded")]
public partial class ExpoHypoDistribution : Distribution
{
    public override double Expected => Rates.Sum(rate => 1.0 / rate);
    
    public override double Mean => Rates.Sum(rate => 1.0 / rate);

    public override double Median => Quantile(0.5);
    
    public override double Mode => GetMode();
    
    public override double Variance => GetVariance();
    
    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => double.PositiveInfinity;

    [EntityParameter(typeof(double[]), nameof(Rates))]
    public double[] Rates { get; private set; } = [2.0, 0.5];
    
    private ExpoDistribution[] _stages;
    
    public override double Distribute() => _stages.Sum(stage => stage.Distribute());

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

        var pdf = 0.0;
        
        for (var i = 0; i < Rates.Length; i++)
        {
            var weight = CalculateStageWeight(i);
            var stagePDF = Rates[i] * Math.Exp(-Rates[i] * x);
            
            pdf += weight * stagePDF;
        }
        
        return pdf;
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;

        var cdf = 0.0;
        
        for (var i = 0; i < Rates.Length; i++)
        {
            var weight = CalculateStageWeight(i);
            var stageCDF = 1 - Math.Exp(-Rates[i] * x);
            
            cdf += weight * stageCDF;
        }
        
        return cdf;
    }

    private double CalculateStageWeight(int stageIndex)
    {
        var stageMean = 1.0 / Rates[stageIndex];
        var totalMean = Rates.Sum(rate => 1.0 / rate);
        
        return stageMean / totalMean;
    }
    
    public int StageCount => Rates.Length;

    public double CoefficientOfVariation
    {
        get
        {
            var variance = Variance;
            var mean = Mean;
            return Math.Sqrt(variance) / mean;
        }
    }

    public override string ToString()
    {
        var ratesString = string.Join(", ", Rates);
        
        return $"Expo Hypo Distribution [Rates = [{ratesString}]]";
    }

    public override bool Equals(object? obj) => obj is ExpoHypoDistribution other && Rates.SequenceEqual(other.Rates);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        
        foreach (var rate in Rates) 
            hash.Add(rate);
        
        return hash.ToHashCode();
    }

    protected override void Validate()
    {
        if (Rates.Length == 0)
            throw new ArgumentException("Rates array cannot be empty", nameof(Rates));
        
        if (Rates.Any(rate => rate <= 0))
            throw new ArgumentException("All rates must be positive", nameof(Rates));
        
        _stages = Rates.Select(rate =>
        {
           var expo = new ExpoDistribution();
           
           expo.Set(rate);

           return expo;

        }).ToArray();
    }

    private double GetMode()
    {
        var maxRate = Rates.Max();
        var index = Array.IndexOf(Rates, maxRate);
        var weight = CalculateStageWeight(index);
            
        return 0;
    }
    
    private double GetVariance()
    {
        var mean = Mean;
        var secondMoment = 0.0;
            
        for (var i = 0; i < Rates.Length; i++)
        {
            var weight = CalculateStageWeight(i);
            var stageVariance = 2.0 / (Rates[i] * Rates[i]);
            secondMoment += weight * stageVariance;
        }
            
        return secondMoment - mean * mean;    
    }
    
    private double GetSkewness()
    {
        var mean = Mean;
        var variance = Variance;
        var thirdMoment = 0.0;
            
        for (var i = 0; i < Rates.Length; i++)
        {
            var weight = CalculateStageWeight(i);
            var stageThirdMoment = 6.0 / (Rates[i] * Rates[i] * Rates[i]);
            thirdMoment += weight * stageThirdMoment;
        }
            
        var numerator = thirdMoment - 3 * mean * variance - mean * mean * mean;
        var denominator = Math.Pow(variance, 1.5);
            
        return numerator / denominator;
    }
    
    private double GetKurtosis()
    {
        var mean = Mean;
        var variance = Variance;
        var secondMoment = variance + mean * mean;
        var fourthMoment = 0.0;
            
        for (var i = 0; i < Rates.Length; i++)
        {
            var weight = CalculateStageWeight(i);
            var stageFourthMoment = 24.0 / (Rates[i] * Rates[i] * Rates[i] * Rates[i]);
            fourthMoment += weight * stageFourthMoment;
        }
            
        var term1 = fourthMoment;
        var term2 = -4 * mean * (Skewness * Math.Pow(variance, 1.5) + 3 * mean * variance + mean * mean * mean);
        var term3 = 6 * mean * mean * secondMoment;
        var term4 = -3 * mean * mean * mean * mean;
            
        var numerator = term1 + term2 + term3 + term4;
        var denominator = variance * variance;
            
        return numerator / denominator;
    }
}