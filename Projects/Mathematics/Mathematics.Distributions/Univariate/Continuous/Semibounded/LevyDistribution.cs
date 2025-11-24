using Mathematics.Core.Base;
using Mathematics.Functions;
using Mathematics.Functions.Transforms;

namespace Mathematics.Distributions.Univariate.Continuous.Semibounded;

public partial class LevyDistribution : Distribution
{
    private readonly double _location;
    private readonly double _scale;
    
    public LevyDistribution(double location = 0, double scale = 1)
    {
        if (scale <= 0)
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be positive");

        _location = location;
        _scale = scale;
    }

    public override double Expected => double.PositiveInfinity;
    
    public override double Mean => double.PositiveInfinity;
    
    public override double Median => _location + _scale / (Math.Pow(InverseNormalCDF(0.75), 2));
    
    public override double Mode => _location + _scale / 3;
    
    public override double Variance => double.PositiveInfinity;
    
    public override double Skewness => double.NaN;
    
    public override double Kurtosis => double.NaN;
    
    public override double StandardDeviation => double.PositiveInfinity;
    
    public override double Minimum => _location;
    
    public override double Maximum => double.PositiveInfinity;

    public double Location => _location;
    
    public double Scale => _scale;
    
    public override double Distribute()
    {
        var (z, _) = BoxMullerPolarTransform.Transform();
        
        return _location + _scale / (z * z);
    }

    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return _location;
            case 1: return double.PositiveInfinity;
            default:
            {
                var z = InverseNormalCDF(1 - p);
                return _location + _scale / (z * z);
            }
        }
    }
    
    public override double ProbabilityDensity(double x)
    {
        if (x <= _location)
            return 0;

        var numerator = Math.Sqrt(_scale / (2 * Math.PI));
        var exponent = -_scale / (2 * (x - _location));
        var denominator = Math.Pow(x - _location, 1.5);
        
        return numerator * Math.Exp(exponent) / denominator;
    }

    public override double CumulativeDistribution(double x)
    {
        if (x <= _location)
            return 0;

        var z = Math.Sqrt(_scale / (x - _location));
        return Erfc(z / Math.Sqrt(2));
    }

    private static double Erfc(double x)
    {
        return 1 - ErrorFunction.Calculate(x);
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
    
    public override string ToString() => $"Levy Distribution [Location = {_location}, Scale = {_scale}]";

    public override bool Equals(object? obj) => obj is LevyDistribution other && _location == other._location && _scale == other._scale;

    public override int GetHashCode() => HashCode.Combine(_location, _scale);
}