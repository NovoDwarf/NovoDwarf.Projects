using Mathematics.Models.Base;
using Mathematics.Utilities;

namespace Mathematics.Models.Distributions.HeavyTailed;

public class Pareto : DistributionBase
{
    public double Scale { get; }
    public double Shape { get; }

    public Pareto(double scale, double shape)
    {
        Scale = scale;
        Shape = shape;
    }

    public override double Calculate()
    {
        var u = RandomUtils.NextDouble();
        
        return Scale / Math.Pow(u, 1.0 / Shape);
    }

    public override double GetExpectedValue() => 
        Shape > 1 ? Shape * Scale / (Shape - 1) : double.PositiveInfinity;
    public override double GetVariance() => 
        Shape > 2 ? Scale * Scale * Shape / ((Shape - 1) * (Shape - 1) * (Shape - 2)) : double.PositiveInfinity;
    public override double GetMinValue() => Scale;
    public override double GetMaxValue() => double.PositiveInfinity;

    public override string ToString() => $"Pareto [Scale = {Scale:F3}, Shape = {Shape:F3}]";
}