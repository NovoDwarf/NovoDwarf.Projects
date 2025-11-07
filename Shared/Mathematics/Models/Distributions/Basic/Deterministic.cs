using Mathematics.Models.Base;

namespace Mathematics.Models.Distributions.Basic;

public class Deterministic : DistributionBase
{
    public Deterministic(double constant)
    {
        Constant = constant;
    }

    public double Constant { get; }
    
    public override double Calculate() => Constant;

    public override double GetExpectedValue() => Constant;
    public override double GetVariance() => 0;
    public override double GetMinValue() => Constant;
    public override double GetMaxValue() => Constant;

    public override string ToString() => $"Deterministic [Constant = {Constant:F3}]";
}