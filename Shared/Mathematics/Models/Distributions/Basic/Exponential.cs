using Mathematics.Models.Base;
using Mathematics.Utilities;

namespace Mathematics.Models.Distributions.Basic;

public class Exponential : DistributionBase
{
	public Exponential(double rates)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(rates, 0);
		
		Rates = rates;
	}

	public double Rates { get; }

	public override double Calculate()
	{
		var u = RandomUtils.NextDoubleSafe();

		return -Math.Log(u) / Rates;
	}

	public override double GetExpectedValue() => 1.0 / Rates;
	public override double GetVariance() => 1.0 / (Rates * Rates);
	public override double GetMinValue() => 0;
	public override double GetMaxValue() => double.PositiveInfinity;

	public override string ToString() => $"Exponential [Lambda = {Rates:F3}]";
}