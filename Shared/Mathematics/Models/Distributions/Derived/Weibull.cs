using Mathematics.Models.Base;
using Mathematics.Utilities;
using Mathematics.Utilities.Formulas;

namespace Mathematics.Models.Distributions.Derived;

public class Weibull : DistributionBase
{
    public double Scale { get; }
    public double Shape { get; }

    public Weibull(double scale, double shape)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(scale, 0);
        ArgumentOutOfRangeException.ThrowIfLessThan(shape, 0);
        
        Scale = scale;
        Shape = shape;
    }
    
    public override double Calculate()
    {
        var u = RandomUtils.NextDoubleSafe();
        
        return Scale * Math.Pow(-Math.Log(u), 1.0 / Shape);
    }

    public override double GetExpectedValue() => Scale * GammaUtils.Gamma(1.0 + 1.0 / Shape);
    
    public override double GetVariance() 
    {
        var gamma1 = GammaUtils.Gamma(1.0 + 1.0 / Shape);
        var gamma2 = GammaUtils.Gamma(1.0 + 2.0 / Shape);
        
        return Scale * Scale * (gamma2 - gamma1 * gamma1);
    }

    public override double GetMinValue() => 0;
    public override double GetMaxValue() => double.PositiveInfinity;

    public override string ToString() => $"Weibull [Scale = {Scale:F3}, Shape = {Shape:F3}]";
}