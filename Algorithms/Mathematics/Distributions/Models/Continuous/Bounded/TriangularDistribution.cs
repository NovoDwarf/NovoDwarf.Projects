using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Continuous.Bounded;

public class TriangularDistribution : DistributionBase
{
	public TriangularDistribution(double min, double max, double mode)
	{
		Min = min;
		Max = max;
		Mode = mode;
	}

	public double Min { get; }
	public double Max { get; }
	public double Mode { get; }

	public override double Calculate()
	{
		var u = RandomUtils.NextDouble();
		var fc = (Mode - Min) / (Max - Min);

		return u < fc
			? Min + Math.Sqrt(u * (Max - Min) * (Mode - Min))
			: Max - Math.Sqrt((1 - u) * (Max - Min) * (Max - Mode));
	}

	public override double GetExpectedValue() => (Min + Max + Mode) / 3.0;

	public override double GetVariance() => (Math.Pow(Min, 2) + Math.Pow(Max, 2) + Math.Pow(Mode, 2) - Min * Max - Min * Mode - Max * Mode) / 18.0;

	public override double GetMinValue() => Min;

	public override double GetMaxValue() => Max;

	public override string ToString() => $"Triangular [Min = {Min:F3}, Max = {Max:F3}, Mode = {Mode:F3}]";
}