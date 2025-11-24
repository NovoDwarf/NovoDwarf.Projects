using Mathematics.Core.Base;

namespace Mathematics.Distributions.Univariate.Continuous.Semibounded;

public partial class ExpoHypoDistribution : Distribution
{
    private readonly ExpoDistribution[] _stages;
    private readonly double[] _rates;
    
    public ExpoHypoDistribution(double[]? rates = null)
    {
        rates ??= [2.0, 0.5];
        
        if (rates.Length == 0)
            throw new ArgumentException("Rates array cannot be empty", nameof(rates));
        
        if (rates.Any(rate => rate <= 0))
            throw new ArgumentException("All rates must be positive", nameof(rates));

        _rates = rates;
        _stages = rates.Select(rate => new ExpoDistribution(rate)).ToArray();
    }

    public override double Expected => _rates.Sum(rate => 1.0 / rate);
    
    public override double Mean => _rates.Sum(rate => 1.0 / rate);
    
    /// <summary>
    /// Медиана гиперэкспоненциального распределения не имеет простой аналитической формы
    /// Используем численный метод для нахождения медианы
    /// </summary>
    public override double Median => Quantile(0.5);

    /// <summary>
    /// Мода соответствует стадии с наибольшей плотностью в нуле
    /// Для гиперэкспоненциального распределения это стадия с наибольшей скоростью
    /// Мода находится путем максимизации плотности
    /// Для простоты возвращаем приближенное значение
    /// </summary>
    public override double Mode => GetMode();
    
    public override double Variance => GetVariance();
    
    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => double.PositiveInfinity;

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
        
        for (var i = 0; i < _rates.Length; i++)
        {
            var weight = CalculateStageWeight(i);
            var stagePDF = _rates[i] * Math.Exp(-_rates[i] * x);
            
            pdf += weight * stagePDF;
        }
        
        return pdf;
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;

        var cdf = 0.0;
        
        for (var i = 0; i < _rates.Length; i++)
        {
            var weight = CalculateStageWeight(i);
            var stageCDF = 1 - Math.Exp(-_rates[i] * x);
            
            cdf += weight * stageCDF;
        }
        
        return cdf;
    }

    private double CalculateStageWeight(int stageIndex)
    {
        var stageMean = 1.0 / _rates[stageIndex];
        var totalMean = _rates.Sum(rate => 1.0 / rate);
        
        return stageMean / totalMean;
    }
    
    public double[] Rates => _rates.ToArray();
    
    public int StageCount => _rates.Length;

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
        var ratesString = string.Join(", ", _rates);
        
        return $"Expo Hypo Distribution [Rates = [{ratesString}]]";
    }

    public override bool Equals(object obj) => obj is ExpoHypoDistribution other && _rates.SequenceEqual(other._rates);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        
        foreach (var rate in _rates) 
            hash.Add(rate);
        
        return hash.ToHashCode();
    }
    
    private double GetMode()
    {
        var maxRate = _rates.Max();
        var index = Array.IndexOf(_rates, maxRate);
        var weight = CalculateStageWeight(index);
            
        return 0;
    }
    
    private double GetVariance()
    {
        var mean = Mean;
        var secondMoment = 0.0;
            
        for (var i = 0; i < _rates.Length; i++)
        {
            var weight = CalculateStageWeight(i);
            var stageVariance = 2.0 / (_rates[i] * _rates[i]);
            secondMoment += weight * stageVariance;
        }
            
        return secondMoment - mean * mean;    
    }
    
    private double GetSkewness()
    {
        var mean = Mean;
        var variance = Variance;
        var thirdMoment = 0.0;
            
        for (var i = 0; i < _rates.Length; i++)
        {
            var weight = CalculateStageWeight(i);
            var stageThirdMoment = 6.0 / (_rates[i] * _rates[i] * _rates[i]);
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
            
        for (var i = 0; i < _rates.Length; i++)
        {
            var weight = CalculateStageWeight(i);
            var stageFourthMoment = 24.0 / (_rates[i] * _rates[i] * _rates[i] * _rates[i]);
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