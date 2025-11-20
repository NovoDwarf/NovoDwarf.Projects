using Mathematics.Core.Distributions.Base;
using Mathematics.Core.Randoms.Utilities;
using Mathematics.Core.Transforms.Models;

namespace Mathematics.Core.Distributions.Models.Univariate.Continuous.SemiInfinite;

public partial class GammaDistribution : Distribution
{
	private readonly double _shape;
	private readonly double _scale;
	
	public GammaDistribution(double shape, double scale)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(shape, 0);
		ArgumentOutOfRangeException.ThrowIfLessThan(scale, 0);

		_shape = shape;
		_scale = scale;
	}

	public override double Expected { get; }
	public override double Mean { get; }
	public override double Median { get; }
	public override double Mode { get; }
	public override double Variance { get; }
	public override double Skewness { get; }
	public override double Kurtosis { get; }
	public override double StandardDeviation { get; }
	public override double Minimum { get; }
	public override double Maximum { get; }

	public override double Distribute()
	{
		return _shape >= 1.0
			? CalculateGamma(_shape) * _scale
			: CalculateSmallShapeGamma(_shape) * _scale;
	}

	public override double ProbabilityDensity(double x)
	{
		throw new NotImplementedException();
	}

	public override double CumulativeDistribution(double x)
	{
		throw new NotImplementedException();
	}

	private double CalculateGamma(double alpha)
	{
		var d = alpha - 1.0 / 3.0;
		var c = 1.0 / Math.Sqrt(9.0 * d);

		while (true)
		{
			double x;
			do
			{
				x = BoxMullerTransform.Polar().u;
			} while (x <= -1.0 / c);

			var v = 1.0 + c * x;
			v = v * v * v;

			var u = RandomUtils.NextDouble();

			if (u < 1.0 - 0.0331 * Math.Pow(x, 4) ||
			    Math.Log(u) < 0.5 * x * x + d * (1.0 - v + Math.Log(v)))
				return d * v;
		}
	}

	private double CalculateSmallShapeGamma(double alpha)
	{
		while (true)
		{
			var u = RandomUtils.NextDouble();
			var v = RandomUtils.NextDouble();

			var x = Math.Pow(u, 1.0 / alpha);
			var y = Math.Pow(v, 1.0 / (1.0 - alpha));

			if (!(x + y <= 1.0))
				continue;

			var z = x / (x + y);
			var w = -Math.Log(Random.Shared.NextDouble());

			return z * w;
		}
	}
}