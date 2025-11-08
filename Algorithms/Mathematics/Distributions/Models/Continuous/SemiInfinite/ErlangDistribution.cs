using Mathematics.Distributions.Base;

namespace Mathematics.Distributions.Models.Continuous.SemiInfinite;

public class ErlangDistribution : DistributionBase
{
	public ErlangDistribution(int shape, double rate)
	{
		if (shape < 1)
			throw new ArgumentException("Shape must be greater than 0.", nameof(shape));

		if (rate <= 0)
			throw new ArgumentException("Rate must be greater than 0.", nameof(rate));

		Shape = shape;
		Rate = rate;
	}

	public int Shape { get; }
	public double Rate { get; }

	public override double Calculate()
	{
		var sum = 0.0;
		var exp = new ExpoDistribution(Rate);

		for (var i = 0; i < Shape; i++)
			sum += exp.Calculate();

		return sum;
	}

	public override double GetExpectedValue()
	{
		return Shape / Rate;
	}

	public override double GetVariance()
	{
		return Shape / (Rate * Rate);
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
		return $"Erlang [Shape = {Shape}, Rate = {Rate:F3}]";
	}
}