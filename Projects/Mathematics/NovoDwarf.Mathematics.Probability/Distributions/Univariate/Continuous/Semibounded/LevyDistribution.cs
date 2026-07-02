using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Numerical.Simple;
using Mathematics.Numerical.Transforms;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

[Categories("Distributions", "Univariate", "Continious", "Semibounded")]
public partial class LevyDistribution : Distribution
{
    public override double Expected => double.PositiveInfinity;
    
    public override double Mean => double.PositiveInfinity;
    
    public override double Median => Location + Scale / (Math.Pow(InverseNormalCDF(0.75), 2));
    
    public override double Mode => Location + Scale / 3;
    
    public override double Variance => double.PositiveInfinity;
    
    public override double Skewness => double.NaN;
    
    public override double Kurtosis => double.NaN;
    
    public override double StandardDeviation => double.PositiveInfinity;
    
    public override double Minimum => Location;
    
    public override double Maximum => double.PositiveInfinity;

    public double Location { get; private set; } = 0;

    public double Scale { get; private set; } = 1;

    protected override void Validate()
    {
        if (Scale <= 0)
            throw new ArgumentOutOfRangeException(nameof(Scale), "Scale must be positive");
    }

    public override double Distribute()
    {
        var (z, _) = BoxMullerPolarTransform.Transform();
        
        return Location + Scale / (z * z);
    }

    public override double Quantile(double p)
    {
        if (p <= 0 || p >= 1)
            throw new ArgumentOutOfRangeException(nameof(p), "Probability must be between 0 and 1");

        switch (p)
        {
            case 0: return Location;
            case 1: return double.PositiveInfinity;
            default:
            {
                var z = InverseNormalCDF(1 - p);
                return Location + Scale / (z * z);
            }
        }
    }
    
    public override double ProbabilityDensity(double x)
    {
        if (x <= Location)
            return 0;

        var numerator = Math.Sqrt(Scale / (2 * Math.PI));
        var exponent = -Scale / (2 * (x - Location));
        var denominator = Math.Pow(x - Location, 1.5);
        
        return numerator * Math.Exp(exponent) / denominator;
    }

    public override double CumulativeDistribution(double x)
    {
        if (x <= Location)
            return 0;

        var z = Math.Sqrt(Scale / (x - Location));
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
    
    public override string ToString() => $"Levy Distribution [Location = {Location}, Scale = {Scale}]";

    public override bool Equals(object? obj) => obj is LevyDistribution other && Location == other.Location && Scale == other.Scale;

    public override int GetHashCode() => HashCode.Combine(Location, Scale);
}