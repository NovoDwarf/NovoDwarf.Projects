using Mathematics.Distributions.Base;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class ParetoDistribution : Distribution
{
	public ParetoDistribution(double scale, double shape)
	{
		Scale = scale;
		Shape = shape;
	}

	public double Scale { get; }
	public double Shape { get; }

	public override double Calculate()
	{
		var u = RandomUtils.NextDouble();

		return Scale / Math.Pow(u, 1.0 / Shape);
	}

	public override double GetProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetCumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	public override double GetExpectedValue()
	{
		return Shape > 1 ? Shape * Scale / (Shape - 1) : double.PositiveInfinity;
	}

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
		return Shape > 2 ? Scale * Scale * Shape / ((Shape - 1) * (Shape - 1) * (Shape - 2)) : double.PositiveInfinity;
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

	public override double GetMinValue()
	{
		return Scale;
	}

	public override double GetMaxValue()
	{
		return double.PositiveInfinity;
	}

	public override string ToString()
	{
		return $"Pareto [Scale = {Scale:F3}, Shape = {Shape:F3}]";
	}
}