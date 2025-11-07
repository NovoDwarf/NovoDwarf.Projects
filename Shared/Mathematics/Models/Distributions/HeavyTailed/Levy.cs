using Mathematics.Models.Base;
using Mathematics.Utilities.Formulas;

namespace Mathematics.Models.Distributions.HeavyTailed;

public class Levy : DistributionBase
{
    public double Location { get; }
    public double Scale { get; }

    public Levy(double location, double scale)
    {
        Location = location;
        Scale = scale;
    }
    
    public override double Calculate()
    {
        var (z, _) = BoxMullerUtils.BoxMullerPolar();
        
        return Location + Scale / (z * z);
    }

    public override double GetExpectedValue() => double.PositiveInfinity;
    public override double GetVariance() => double.PositiveInfinity;
    public override double GetMinValue() => Location;
    public override double GetMaxValue() => double.PositiveInfinity;

    public override string ToString() => $"Levy [Location = {Location:F3}, Scale = {Scale:F3}]";
}