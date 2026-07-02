using System.ComponentModel.DataAnnotations;
using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Numerical.Simple;
using Mathematics.Numerical.Transforms;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

[Categories("Distributions", "Univariate", "Continious", "Semibounded")]
public partial class NormalTruncatedDistribution : Distribution
{
    public override double Expected => GetMean();
    
    public override double Mean => GetMean();
    
    public override double Median => Quantile(0.5);
    
    public override double Mode => GetMode();
    
    public override double Variance => GetVariance();
    
    public override double Skewness => GetSkewness();
    
    public override double Kurtosis => GetKurtosis();
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => _minimum;
    
    public override double Maximum => _maximum;

    [EntityParameter(typeof(double), nameof(Mean))]
    [Range(double.NegativeInfinity, double.PositiveInfinity)]
    private double _mean = 0;
    
    [EntityParameter(typeof(double), nameof(StandardDeviation))]
    [Range(0, double.PositiveInfinity)]
    private double _standardDeviation = 1;
    
    [EntityParameter(typeof(double), nameof(Minimum))]
    [Range(double.NegativeInfinity, double.PositiveInfinity)]
    private double _minimum = 0;
    
    [EntityParameter(typeof(double), nameof(Maximum))]
    [Range(double.NegativeInfinity, double.PositiveInfinity)]
    private double _maximum = 1;

    protected override void Validate()
    {
        if (_standardDeviation <= 0)
            throw new ArgumentOutOfRangeException(nameof(_standardDeviation), "Standard deviation must be positive");
        
        if (_minimum >= _maximum)
            throw new ArgumentOutOfRangeException(nameof(_minimum), "Min must be less than max");
    }

    public override double Distribute()
    {
        double x;
        do
        {
            var (z, _) = BoxMullerPolarTransform.Transform();
            x = _mean + _standardDeviation * z;
        } while (x < _minimum || x > _maximum);

        return x;
    }
    
    public override double Quantile(double p)
    {
        if (p <= 0) return _minimum;
        if (p >= 1) return _maximum;

        var normalCDFMin = NormalCDF(_minimum, _mean, _standardDeviation);
        var normalCDFMax = NormalCDF(_maximum, _mean, _standardDeviation);
        
        var quantileNormal = normalCDFMin + p * (normalCDFMax - normalCDFMin);
        return _mean + _standardDeviation * InverseNormalCDF(quantileNormal);
    }

    public override double ProbabilityDensity(double x)
    {
        if (x < _minimum || x > _maximum)
            return 0;

        var normalPDF = NormalPDF(x, _mean, _standardDeviation);
        var normalCDFMin = NormalCDF(_minimum, _mean, _standardDeviation);
        var normalCDFMax = NormalCDF(_maximum, _mean, _standardDeviation);
        
        return normalPDF / (normalCDFMax - normalCDFMin);
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < _minimum)
            return 0;
        if (x > _maximum)
            return 1;

        var normalCDFX = NormalCDF(x, _mean, _standardDeviation);
        var normalCDFMin = NormalCDF(_minimum, _mean, _standardDeviation);
        var normalCDFMax = NormalCDF(_maximum, _mean, _standardDeviation);
        
        return (normalCDFX - normalCDFMin) / (normalCDFMax - normalCDFMin);
    }

    // Вспомогательные методы для нормального распределения
    private static double NormalPDF(double x, double mean, double stdDev)
    {
        var exponent = -0.5 * Math.Pow((x - mean) / stdDev, 2);
        return Math.Exp(exponent) / (stdDev * Math.Sqrt(2 * Math.PI));
    }

    private static double NormalCDF(double x, double mean, double stdDev)
    {
        var z = (x - mean) / (stdDev * Math.Sqrt(2));
        return 0.5 * (1 + ErrorFunction.Calculate(z));
    }
    
    private static double InverseNormalCDF(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p));

        double[] a = { -3.969683028665376e+01, 2.209460984245205e+02,
                      -2.759285104469687e+02, 1.383577518672690e+02,
                      -3.066479806614716e+01, 2.506628277459239e+00 };

        double[] b = { -5.447609879822406e+01, 1.615858368580409e+02,
                      -1.556989798598866e+02, 6.680131188771972e+01,
                      -1.328068155288572e+01 };

        double[] c = { -7.784894002430293e-03, -3.223964580411365e-01,
                      -2.400758277161838e+00, -2.549732539343734e+00,
                      4.374664141464968e+00, 2.938163982698783e+00 };

        double[] d = { 7.784695709041462e-03, 3.224671290700398e-01,
                      2.445134137142996e+00, 3.754408661907416e+00 };

        double q, r;

        if (p < 0.02425)
        {
            q = Math.Sqrt(-2 * Math.Log(p));
            return (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
                   ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
        }
        else if (p > 0.97575)
        {
            q = Math.Sqrt(-2 * Math.Log(1 - p));
            return -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
                    ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
        }
        else
        {
            q = p - 0.5;
            r = q * q;
            return (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q /
                   (((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1);
        }
    }

    public override string ToString() => $"Normal Truncated Distribution [Mean = {_mean}, StdDev = {_standardDeviation}, Min = {_minimum}, Max = {_maximum}]";

    public override bool Equals(object obj) => obj is NormalTruncatedDistribution other &&
                                               _mean == other._mean && _standardDeviation == other._standardDeviation &&
                                               _minimum == other._minimum && _maximum == other._maximum;

    public override int GetHashCode() => HashCode.Combine(_mean, _standardDeviation, _minimum, _maximum);
    
    private double GetMean()
    {
        var alpha = (_minimum - _mean) / _standardDeviation;
        var beta = (_maximum - _mean) / _standardDeviation;
        
        var z = NormalCDF(beta, 0, 1) - NormalCDF(alpha, 0, 1);
        var phiAlpha = NormalPDF(alpha, 0, 1);
        var phiBeta = NormalPDF(beta, 0, 1);
        
        return _mean + _standardDeviation * (phiAlpha - phiBeta) / z;
    }

    private double GetMode()
    {
        if (_mean >= _minimum && _mean <= _maximum)
            return _mean;
            
        if (_mean < _minimum)
            return _minimum;
            
        return _maximum;
    }
    
    private double GetVariance()
    {
        var alpha = (_minimum - _mean) / _standardDeviation;
        var beta = (_maximum - _mean) / _standardDeviation;
        
        var z = NormalCDF(beta, 0, 1) - NormalCDF(alpha, 0, 1);
        var phiAlpha = NormalPDF(alpha, 0, 1);
        var phiBeta = NormalPDF(beta, 0, 1);
        
        var term1 = (alpha * phiAlpha - beta * phiBeta) / z;
        var term2 = Math.Pow((phiAlpha - phiBeta) / z, 2);
        
        return _standardDeviation * _standardDeviation * (1 + term1 - term2);
    }

    private double GetSkewness()
    {
        var alpha = (_minimum - _mean) / _standardDeviation;
        var beta = (_maximum - _mean) / _standardDeviation;
        
        var z = NormalCDF(beta, 0, 1) - NormalCDF(alpha, 0, 1);
        var phiAlpha = NormalPDF(alpha, 0, 1);
        var phiBeta = NormalPDF(beta, 0, 1);
        
        var delta = (phiAlpha - phiBeta) / z;
        var gamma = (alpha * phiAlpha - beta * phiBeta) / z;
        
        var term1 = (alpha * alpha * phiAlpha - beta * beta * phiBeta) / z;
        var term2 = 3 * gamma;
        var term3 = 2 * delta * delta * delta;
        
        var numerator = term1 - term2 + term3;
        var denominator = Math.Pow(1 + gamma - delta * delta, 1.5);
        
        return numerator / denominator;
    }

    private double GetKurtosis()
    {
        var alpha = (_minimum - _mean) / _standardDeviation;
        var beta = (_maximum - _mean) / _standardDeviation;
        
        var z = NormalCDF(beta, 0, 1) - NormalCDF(alpha, 0, 1);
        var phiAlpha = NormalPDF(alpha, 0, 1);
        var phiBeta = NormalPDF(beta, 0, 1);
        
        var delta = (phiAlpha - phiBeta) / z;
        var gamma = (alpha * phiAlpha - beta * phiBeta) / z;
        
        var term1 = (alpha * alpha * alpha * phiAlpha - beta * beta * beta * phiBeta) / z;
        var term2 = 4 * (alpha * alpha * phiAlpha - beta * beta * phiBeta) / z;
        var term3 = 6 * (alpha * phiAlpha - beta * phiBeta) / z;
        var term4 = 3 * (1 + gamma - delta * delta) * (1 + gamma - delta * delta);
        var term5 = 6 * delta * delta * (1 + gamma);
        var term6 = 4 * delta * delta * delta * delta;
        
        var numerator = term1 - term2 + term3 + term4 - term5 + term6;
        var denominator = Math.Pow(1 + gamma - delta * delta, 2);
        
        return numerator / denominator - 3;
    }
}