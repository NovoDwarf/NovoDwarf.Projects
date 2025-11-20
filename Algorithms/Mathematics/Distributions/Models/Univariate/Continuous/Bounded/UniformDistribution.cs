using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Continuous.Bounded;

public partial class UniformDistribution : Distribution
{
	public UniformDistribution(double min, double max)
	{
		Min = min;
		Max = max;
	}

	public double Min { get; }
	public double Max { get; }

	/// <inheritdoc cref="Distribution.Calculate()"/>
	public override double Calculate() => RandomUtils.NextDouble(Min, Max);

	public override double GetProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetCumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc cref="Distribution.GetExpectedValue()"/>
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

	/// <inheritdoc cref="Distribution.GetVariance()"/>
	public override double GetVariance() => Math.Pow(Max - Min, 2) / 12.0;

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

	/// <inheritdoc cref="Distribution.GetMinValue()"/>
	public override double GetMinValue() => Min;

	/// <inheritdoc cref="Distribution.GetMaxValue()"/>
	public override double GetMaxValue() => Max;

	/// <inheritdoc cref="Distribution.ToString()"/>
	public override string ToString() => $"Uniform [Min = {Min:F3}, Max = {Max:F3})";
}