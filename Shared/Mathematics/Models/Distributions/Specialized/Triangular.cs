using Mathematics.Models.Base;
using Mathematics.Utilities;

namespace Mathematics.Models.Distributions.Specialized;

public class Triangular : DistributionBase
{
    public double Min { get; }
    public double Max { get; }
    public double Mode { get; }

    public Triangular(double min, double max, double mode)
    {
        Min = min;
        Max = max;
        Mode = mode;
    }

    public override double Calculate()
    {
        var u = RandomUtils.NextDouble();
        var fc = (Mode - Min) / (Max - Min);

        return u < fc
            ? Min + Math.Sqrt(u * (Max - Min) * (Mode - Min))
            : Max - Math.Sqrt((1 - u) * (Max - Min) * (Max - Mode));
    }

    public override double GetExpectedValue() => (Min + Max + Mode) / 3.0;
    public override double GetVariance() => 
        (Math.Pow(Min, 2) + Math.Pow(Max, 2) + Math.Pow(Mode, 2) - Min * Max - Min * Mode - Max * Mode) / 18.0;
    public override double GetMinValue() => Min;
    public override double GetMaxValue() => Max;

    public override string ToString() => $"Triangular [Min = {Min:F3}, Max = {Max:F3}, Mode = {Mode:F3}]";
}