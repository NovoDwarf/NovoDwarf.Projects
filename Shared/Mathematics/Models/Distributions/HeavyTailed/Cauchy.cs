using Mathematics.Models.Base;
using Mathematics.Utilities;

namespace Mathematics.Models.Distributions.HeavyTailed;

public class Cauchy : DistributionBase
{
    public double Location { get; }
    public double Scale { get; }

    public Cauchy(double location, double scale)
    {
        Location = location;
        Scale = scale;
    }

    public override double Calculate()
    {
        var u = RandomUtils.NextDouble();
        
        return Location + Scale * Math.Tan(Math.PI * (u - 0.5));
    }

    public override double GetExpectedValue() => double.NaN; 
    public override double GetVariance() => double.NaN;
    public override double GetMinValue() => double.NegativeInfinity;
    public override double GetMaxValue() => double.PositiveInfinity;

    public override string ToString() => $"Cauchy [Location = {Location:F3}, Scale = {Scale:F3}]";
}