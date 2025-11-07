using Mathematics.Models.Base;
using Mathematics.Utilities;
using Mathematics.Utilities.Formulas;

namespace Mathematics.Models.Distributions.Composite;

public class Gamma : DistributionBase
{
    public double Shape { get; }
    public double Scale { get; }

    public Gamma(double shape, double scale)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(shape, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(scale, 0);
        
        Shape = shape;
        Scale = scale;
    }

    public override double Calculate()
    {
        return Shape >= 1.0 
            ? CalculateGamma(Shape) * Scale 
            : CalculateSmallShapeGamma(Shape) * Scale;
    }
    
    public override double GetExpectedValue() => Shape * Scale;
    public override double GetVariance() => Shape * Scale * Scale;
    public override double GetMinValue() => 0;
    public override double GetMaxValue() => double.PositiveInfinity;

    public override string ToString() => $"Gamma [Shape = {Shape:F3}, Scale = {Scale:F3}]";
    
    private double CalculateGamma(double alpha)
    {
        var d = alpha - 1.0 / 3.0;
        var c = 1.0 / Math.Sqrt(9.0 * d);

        while (true)
        {
            double x;
            do
            {
                x = BoxMullerUtils.BoxMullerPolar().u;
            } while (x <= -1.0 / c);
            
            var v = 1.0 + c * x;
            v = v * v * v;
            
            var u = RandomUtils.NextDouble();

            if (u < 1.0 - 0.0331 * Math.Pow(x, 4) || 
                Math.Log(u) < 0.5 * x * x + d * (1.0 - v + Math.Log(v)))
            {
                return d * v;
            }
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
}