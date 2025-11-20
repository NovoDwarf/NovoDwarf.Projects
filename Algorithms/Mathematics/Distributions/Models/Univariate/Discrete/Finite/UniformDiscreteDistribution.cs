using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Discrete.Finite;

public partial class UniformDiscreteDistribution : Distribution
{
	public UniformDiscreteDistribution(int min, int max)
	{
		if (min >= max)
			throw new ArgumentOutOfRangeException(nameof(min), "Minimum value must be less than maximum value.");

		Min = min;
		Max = max;
	}

	public int Min { get; }
	public int Max { get; }

	public override double Calculate()
	{
		return RandomUtils.Next(Min, Max + 1);
	}

	public override double GetProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetCumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetExpectedValue() => (Min + Max) / 2.0;
	public override double GetMean()
	{
		throw new NotImplementedException();
	}

	public override double GetMedian()
	{
		throw new NotImplementedException();
	}

	public override double GetMode()
	{
		throw new NotImplementedException();
	}

	public override double GetVariance()
	{
		var n = Max - Min + 1;
		
		return (n * n - 1) / 12.0;
	}

	public override double GetSkewness()
	{
		throw new NotImplementedException();
	}

	public override double GetKurtosis()
	{
		throw new NotImplementedException();
	}

	public override double GetStandardDeviation()
	{
		throw new NotImplementedException();
	}

	public override double GetMinValue() => Min;

	public override double GetMaxValue() => Max;
}