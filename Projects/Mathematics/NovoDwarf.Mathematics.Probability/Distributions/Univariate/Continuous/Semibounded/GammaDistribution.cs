using Mathematics.Core.Attributes;
using Mathematics.Core.Base.Entities;
using Mathematics.Core.Utilities;
using Mathematics.Numerical.Simple;
using Mathematics.Numerical.Transforms;

namespace Mathematics.Probability.Distributions.Univariate.Continuous.Semibounded;

[Categories("Distributions", "Univariate", "Continious", "Semibounded")]
public partial class GammaDistribution : Distribution
{
    public override double Expected => Shape * Scale;
    
    public override double Mean => Shape * Scale;
    
    public override double Median => DoubleUtils.Approximately(Scale, 1) ? Scale * Math.Log(2) : Scale * (Shape - 1.0 / 3.0);

    public override double Mode => Shape >= 1 ? (Shape - 1) * Scale : 0;

    public override double Variance => Shape * Scale * Scale;
    
    public override double Skewness => 2.0 / Math.Sqrt(Shape);
    
    public override double Kurtosis => 6.0 / Shape;
    
    public override double StandardDeviation => Math.Sqrt(Variance);
    
    public override double Minimum => 0;
    
    public override double Maximum => double.PositiveInfinity;

    [EntityParameter(typeof(double), nameof(Shape))]
    public double Shape { get; set; } = 2;

    [EntityParameter(typeof(double), nameof(Scale))]
    public double Scale { get; set; } = 1;

    protected override void Validate()
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(Shape, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(Scale, 0);
    }
    
    public override double Distribute()
    {
        return Shape >= 1.0
            ? CalculateGamma(Shape) * Scale
            : CalculateSmallShapeGamma(Shape) * Scale;
    }

    public override double Quantile(double p)
    {
        if (p < 0 || p > 1)
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

        if (x == 0)
        {
            return Shape switch
            {
                < 1 => double.PositiveInfinity,
                1 => 1.0 / Scale,
                _ => 0
            };
        }

        var numerator = Math.Pow(x, Shape - 1) * Math.Exp(-x / Scale);
        var denominator = GammaFunction.Calculate(Shape) * Math.Pow(Scale, Shape);
        
        return numerator / denominator;
    }

    public override double CumulativeDistribution(double x)
    {
        if (x < 0)
            return 0;

        return LowerIncompleteGamma(Shape, x / Scale) / GammaFunction.Calculate(Shape);
    }
    
    private static double LowerIncompleteGamma(double s, double x)
    {
        if (x == 0) 
            return 0;
        if (double.IsPositiveInfinity(x)) 
            return GammaFunction.Calculate(s);

        const int steps = 1000;
        double sum = 0;
        var step = x / steps;

        for (var i = 0; i < steps; i++)
        {
            var t = (i + 0.5) * step;
            sum += Math.Pow(t, s - 1) * Math.Exp(-t);
        }

        return sum * step;
    }

    private double CalculateGamma(double alpha)
    {
        var d = alpha - 1.0 / 3.0;
        var c = 1.0 / Math.Sqrt(9.0 * d);

        while (true)
        {
            double x;
            do
            {
                x = BoxMullerPolarTransform.Transform().u;
            } while (x <= -1.0 / c);

            var v = 1.0 + c * x;
            v = v * v * v;

            var u = RandomUtils.NextDouble();

            if (u < 1.0 - 0.0331 * Math.Pow(x, 4) ||
                Math.Log(u) < 0.5 * x * x + d * (1.0 - v + Math.Log(v)))
                return d * v;
        }
    }

    private double CalculateSmallShapeGamma(double alpha)
    {
        while (true)
        {
            var u = RandomUtils.NextDouble();
            var v = RandomUtils.NextDouble();

            var x = Math.Pow(u, 1.0 / alpha);
            var y = Math.Pow(v, 1.0 / (1.0 - alpha));

            if (!(x + y <= 1.0))
                continue;

            var z = x / (x + y);
            var w = -Math.Log(Random.Shared.NextDouble());

            return z * w;
        }
    }
    
    public override string ToString() => $"Gamma Distribution [Shape = {Shape}, Scale = {Scale}]";

    public override bool Equals(object? obj) => obj is GammaDistribution other && Shape == other.Shape && Scale == other.Scale;

    public override int GetHashCode() => HashCode.Combine(Shape, Scale);
}