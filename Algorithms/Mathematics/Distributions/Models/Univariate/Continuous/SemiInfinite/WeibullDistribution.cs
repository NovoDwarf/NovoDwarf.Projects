using Mathematics.Distributions.Base;
using Mathematics.Functions.Models;
using Mathematics.Randoms.Utilities;

namespace Mathematics.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class WeibullDistribution : Distribution
{
	public WeibullDistribution(double scale, double shape)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(scale, 0);
		ArgumentOutOfRangeException.ThrowIfLessThan(shape, 0);

		Scale = scale;
		Shape = shape;
	}

	public double Scale { get; }
	public double Shape { get; }

	public override double Calculate()
	{
		var u = RandomUtils.NextDoubleSafe();

		return Scale * Math.Pow(-Math.Log(u), 1.0 / Shape);
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
		return Scale * GammaFunction.Calculate(1.0 + 1.0 / Shape);
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
		var gamma1 = GammaFunction.Calculate(1.0 + 1.0 / Shape);
		var gamma2 = GammaFunction.Calculate(1.0 + 2.0 / Shape);

		return Scale * Scale * (gamma2 - gamma1 * gamma1);
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
		return 0;
	}

	public override double GetMaxValue()
	{
		return double.PositiveInfinity;
	}

	public override string ToString()
	{
		return $"Weibull [Scale = {Scale:F3}, Shape = {Shape:F3}]";
	}
}