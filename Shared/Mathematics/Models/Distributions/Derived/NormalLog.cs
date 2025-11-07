using Mathematics.Models.Base;
using Mathematics.Utilities;

namespace Mathematics.Models.Distributions.Derived;

public class NormalLog : DistributionBase
{
    public double Mean { get; }
    public double StandardDeviation { get; }

    public NormalLog(double mean, double standardDeviation)
    {
        Mean = mean;
        StandardDeviation = standardDeviation;
    }
    
    public override double Calculate()
    {
        var u = RandomUtils.NextNormal();

        return Math.Exp(u);
    }

    public override double GetExpectedValue() => Math.Exp(Mean + StandardDeviation * StandardDeviation / 2.0);
    public override double GetVariance() => 
        Math.Exp(2 * Mean + StandardDeviation * StandardDeviation) * (Math.Exp(StandardDeviation * StandardDeviation) - 1);
    public override double GetMinValue() => 0;
    public override double GetMaxValue() => double.PositiveInfinity;

    public override string ToString() => $"LogNormal [Mean = {Mean:F3}, std={StandardDeviation:F3})";
}