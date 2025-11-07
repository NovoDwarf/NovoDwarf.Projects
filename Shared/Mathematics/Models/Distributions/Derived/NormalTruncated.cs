using Mathematics.Models.Base;
using Mathematics.Utilities.Formulas;

namespace Mathematics.Models.Distributions.Derived;

public class NormalTruncated : DistributionBase
{
    public double Mean { get; }
    public double StandardDeviation { get; }
    public double Min { get; }
    public double Max { get; }

    public NormalTruncated(double mean, double standardDeviation, double min, double max)
    {
        Mean = mean;
        StandardDeviation = standardDeviation;
        Min = min;
        Max = max;
    }
    
    public override double Calculate()
    {
        double x;
        do
        {
            var (z, _) = BoxMullerUtils.BoxMullerPolar();
            x = Mean + StandardDeviation * z;
        } while (x < Min || x > Max);

        return x;
    }

    public override double GetExpectedValue() => double.NaN; // Сложно вычислить аналитически
    public override double GetVariance() => double.NaN; // Сложно вычислить аналитически
    public override double GetMinValue() => Min;
    public override double GetMaxValue() => Max;

    public override string ToString() => $"TruncatedNormal(mean={Mean:F3}, std={StandardDeviation:F3}, min={Min:F3}, max={Max:F3})";
}