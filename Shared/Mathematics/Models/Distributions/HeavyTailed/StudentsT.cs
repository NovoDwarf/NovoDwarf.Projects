using Mathematics.Models.Base;
using Mathematics.Utilities.Formulas;

namespace Mathematics.Models.Distributions.HeavyTailed;

public class StudentsT : DistributionBase
{
    public int DegreesOfFreedom { get; }

    public StudentsT(int degreesOfFreedom)
    {
        DegreesOfFreedom = degreesOfFreedom;
    }

    public override double Calculate()
    {
        var (z, _) = BoxMullerUtils.BoxMullerPolar();

        var v = 0.0;
        
        for (var i = 0; i < DegreesOfFreedom; i++)
        {
            var (zi, _) = BoxMullerUtils.BoxMullerPolar();
            v += zi * zi;
        }

        return z / Math.Sqrt(v / DegreesOfFreedom);
    }

    public override double GetExpectedValue() => 
        DegreesOfFreedom > 1 ? 0 : double.NaN;
    public override double GetVariance() => 
        DegreesOfFreedom > 2 ? DegreesOfFreedom / (double)(DegreesOfFreedom - 2) : double.NaN;
    public override double GetMinValue() => double.NegativeInfinity;
    public override double GetMaxValue() => double.PositiveInfinity;

    public override string ToString() => $"StudentsT [Degrees of Freedom = {DegreesOfFreedom}]";
}