using Mathematics.Distributions.Base;
using Mathematics.Transforms.Models;

namespace Mathematics.Distributions.Models.Continuous.SemiInfinite;

public class NormalTruncatedDistribution : DistributionBase
{
	public NormalTruncatedDistribution(double mean, double standardDeviation, double min, double max)
	{
		Mean = mean;
		StandardDeviation = standardDeviation;
		Min = min;
		Max = max;
	}

	public double Mean { get; }
	public double StandardDeviation { get; }
	public double Min { get; }
	public double Max { get; }

	public override double Calculate()
	{
		double x;
		do
		{
			var (z, _) = BoxMullerTransform.Polar();
			x = Mean + StandardDeviation * z;
		} while (x < Min || x > Max);

		return x;
	}

	public override double GetExpectedValue()
	{
		return double.NaN;
		// Сложно вычислить аналитически
	}

	public override double GetVariance()
	{
		return double.NaN;
		// Сложно вычислить аналитически
	}

	public override double GetMinValue()
	{
		return Min;
	}

	public override double GetMaxValue()
	{
		return Max;
	}

	public override string ToString()
	{
		return $"TruncatedNormal(mean={Mean:F3}, std={StandardDeviation:F3}, min={Min:F3}, max={Max:F3})";
	}
}