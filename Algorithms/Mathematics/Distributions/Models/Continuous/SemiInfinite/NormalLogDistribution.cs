using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Continuous.SemiInfinite;

public class NormalLogDistribution : DistributionBase
{
	public NormalLogDistribution(double mean, double standardDeviation)
	{
		Mean = mean;
		StandardDeviation = standardDeviation;
	}

	public double Mean { get; }
	public double StandardDeviation { get; }

	public override double Calculate()
	{
		var u = RandomUtils.NextNormal();

		return Math.Exp(u);
	}

	public override double GetExpectedValue()
	{
		return Math.Exp(Mean + StandardDeviation * StandardDeviation / 2.0);
	}

	public override double GetVariance()
	{
		return Math.Exp(2 * Mean + StandardDeviation * StandardDeviation) *
		       (Math.Exp(StandardDeviation * StandardDeviation) - 1);
	}

	public override double GetMinValue()
	{
		return 0;
	}

	public override double GetMaxValue()
	{
		return double.PositiveInfinity;
	}

	public override string ToString()
	{
		return $"LogNormal [Mean = {Mean:F3}, std={StandardDeviation:F3})";
	}
}